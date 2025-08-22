using System.Diagnostics.CodeAnalysis;

using CodeGenerator.Application.Settings;
using CodeGenerator.Designer.UI.ViewModels;

using Dapper;

using DataLib;
using DataLib.Extensions;

using Library.CodeGenLib;
using Library.CodeGenLib.Back;
using Library.CodeGenLib.Models;
using Library.Coding;
using Library.Resulting;
using Library.Validations;

using Microsoft.Data.SqlClient;

namespace CodeGenerator.Application.Services;

internal sealed partial class DtoService(SqlConnection connection, ICodeGeneratorEngine<INamespace> codeGenerator, IPropertyService propertyService) : IDtoService, IDisposable
{
    private readonly ICodeGeneratorEngine<INamespace> _codeGenerator = codeGenerator;
    private readonly SqlConnection _connection = connection;
    private readonly IPropertyService _propertyService = propertyService;

    public DtoViewModel CrateByTable(Table table) => new()
    {
        Properties = new(table.Fields.Select(_propertyService.GetByTableField)),
        Name = table.Name,
        ObjectId = table.ObjectId,
        Schema = table.Schema,
        NameSpace = $"{Settings.Settings.Default.CodeConfigs.RootNameSpace}.Dto"
    };

    [return: NotNull]
    public Task<IResult> Delete(long id, CancellationToken ct = default) => CatchResultAsync(async () =>
    {
        _ = await this._connection.ExecuteAsync("DELETE FROM [infra].[Dto] WHERE Id = @Id", new { Id = id });
        await this._propertyService.DeleteByParentId(id, ct).ThrowOnFail().End();
    });

    public void Dispose() =>
        this._connection.Dispose();

    [return: NotNull]
    public IResult<Codes> GenerateCodes(Dto dto, CancellationToken ct = default) => CatchResult(() =>
    {
        Check.MustBeArgumentNotNull(dto, nameof(dto));
        Check.MustBeNotNull(dto.Namespace, nameof(dto.Namespace));
        Check.MustBeNotNull(dto.Name, nameof(dto.Name));

        var className = $"{dto.Name}Dto";
        var nameSpace = INamespace.New(dto.Namespace);
        var classType = new Class(className) { InheritanceModifier = InheritanceModifier.Partial | InheritanceModifier.Sealed };
        _ = nameSpace.AddType(classType);
        var mainClass = this._codeGenerator.Generate(nameSpace, className, Languages.CSharp, false);

        if (!dto.BaseType.IsNullOrEmpty())
        {
            _ = classType.AddBaseType(TypePath.Parse(dto.BaseType!));
        }

        foreach (var prop in dto.Properties)
        {
            var type = TypePath.Parse(prop.TypeFullName ?? "object");
            if (prop.IsList == true)
            {
                type = TypePath.ParseEnumerable(type);
            }

            if (prop.IsNullable == true)
            {
                type = type.WithNullable(true);
            }

            PropertyAccessor? setter = prop.HasSetter is not true ? null : new PropertyAccessor();
            PropertyAccessor? getter = prop.HasGetter is not true ? null : new PropertyAccessor();

            var property = new CodeGenProperty(prop.Name, type, setter: setter, getter: getter);
            _ = classType.AddProperty(property);
        }

        var partClass = this._codeGenerator.Generate(nameSpace, className, Languages.CSharp, true);
        return new Codes(mainClass, partClass);
    });

    [return: NotNull]
    public Task<IResult<IEnumerable<Dto>>> GetAll(CancellationToken ct = default) => CatchResultAsync(() =>
        this._connection.QueryAsync<Dto>("SELECT * FROM [infra].[Dto]"));

    [return: NotNull]
    public Task<IResult<Dto?>> GetById(long id, CancellationToken ct = default) => CatchResultAsync(async () =>
    {
        var dto = await this._connection.QueryFirstOrDefaultAsync<Dto>(
            "SELECT * FROM [infra].[Dto] WHERE Id = @Id",
            new { Id = id });

        if (dto is not null)
        {
            var props = await this._connection.QueryAsync<Property>(
                "SELECT * FROM [infra].[Property] WHERE ParentEntityId = @Id",
                new { dto.Id });
            dto.Properties = [.. props];
        }

        return dto;
    });

    [return: NotNull]
    public Task<IResult<long>> Insert(Dto dto, CancellationToken ct = default) => CatchResultAsync(async () =>
    {
        _ = this.Validate(dto);

        const string dtoSql = """
        INSERT INTO [infra].[Dto]
          (Name, NameSpace, ModuleId, DbObjectId, Comment, IsParamsDto, IsResultDto, IsViewModel, IsList, BaseType)
          VALUES (@Name, @NameSpace, @ModuleId, @DbObjectId, @Comment, @IsParamsDto, @IsResultDto, @IsViewModel, @IsList, @BaseType);
        SELECT CAST(SCOPE_IDENTITY() AS bigint);
        """;

        const string propSql = """
        INSERT INTO [infra].[Property]
          (ParentEntityId, PropertyType, TypeFullName, Name, HasSetter, HasGetter, IsList, IsNullable, Comment, DbObjectId)
          VALUES (@ParentEntityId, @PropertyType, @TypeFullName, @Name, @HasSetter, @HasGetter, @IsList, @IsNullable, @Comment, @DbObjectId);
        """;

        var wasClosed = this._connection.State != System.Data.ConnectionState.Open;
        if (wasClosed)
        {
            await this._connection.OpenAsync(ct);
        }

        using var trans = this._connection.BeginTransaction();
        try
        {
            var dtoId = await this._connection.ExecuteScalarAsync<long>(dtoSql, new
            {
                dto.Name,
                NameSpace = dto.Namespace,
                dto.ModuleId,
                dto.DbObjectId,
                dto.Comment,
                dto.IsParamsDto,
                dto.IsResultDto,
                dto.IsViewModel,
                dto.IsList,
                dto.BaseType,
            }, trans);

            foreach (var prop in dto.Properties)
            {
                _ = await this._connection.ExecuteAsync(propSql, new
                {
                    ParentEntityId = dtoId,
                    prop.PropertyType,
                    prop.TypeFullName,
                    prop.Name,
                    prop.HasSetter,
                    prop.HasGetter,
                    prop.IsList,
                    prop.IsNullable,
                    prop.Comment,
                    prop.DbObjectId,
                }, trans);
            }

            trans.Commit();
            return dtoId;
        }
        catch
        {
            trans.Rollback();
            throw;
        }
        finally
        {
            if (wasClosed)
            {
                await this._connection.CloseAsync();
            }
        }
    });

    [return: NotNull]
    public Task<IResult> Update(long id, Dto dto, CancellationToken ct = default) => CatchResultAsync(async () =>
    {
        this.Validate(dto).ThrowOnFail().End();
        const string dtoSql = """
        UPDATE [infra].[Dto] SET
          Name = @Name,
          NameSpace = @NameSpace,
          ModuleId = @ModuleId,
          DbObjectId = @DbObjectId,
          Comment = @Comment,
          IsParamsDto = @IsParamsDto,
          IsResultDto = @IsResultDto,
          IsViewModel = @IsViewModel,
          IsList = @IsList,
          BaseType = @BaseType
        WHERE Id = @Id
        """;

        const string deletePropSql = "DELETE FROM [infra].[Property] WHERE DtoId = @Id";
        const string propSql = """
        INSERT INTO [infra].[Property]
          (ParentEntityId, PropertyType, TypeFullName, Name, HasSetter, HasGetter, IsList, IsNullable, Comment, DbObjectId, DtoId)
          VALUES (@ParentEntityId, @PropertyType, @TypeFullName, @Name, @HasSetter, @HasGetter, @IsList, @IsNullable, @Comment, @DbObjectId, @DtoId);
        """;

        var wasClosed = this._connection.State != System.Data.ConnectionState.Open;
        if (wasClosed)
        {
            await this._connection.OpenAsync(ct);
        }

        using var trans = this._connection.BeginTransaction();
        try
        {
            _ = await this._connection.ExecuteAsync(dtoSql, new
            {
                dto.Name,
                NameSpace = dto.Namespace,
                dto.ModuleId,
                dto.DbObjectId,
                dto.Comment,
                dto.IsParamsDto,
                dto.IsResultDto,
                dto.IsViewModel,
                dto.IsList,
                dto.BaseType,
                Id = id
            }, trans);

            _ = await this._connection.ExecuteAsync(deletePropSql, new { Id = id }, trans);

            foreach (var prop in dto.Properties)
            {
                _ = await this._connection.ExecuteAsync(propSql, new
                {
                    ParentEntityId = id,
                    prop.PropertyType,
                    prop.TypeFullName,
                    prop.Name,
                    prop.HasSetter,
                    prop.HasGetter,
                    prop.IsList,
                    prop.IsNullable,
                    prop.Comment,
                    prop.DbObjectId,
                    DtoId = id
                }, trans);
            }
            trans.Commit();
        }
        catch
        {
            trans.Rollback();
            throw;
        }
        finally
        {
            if (wasClosed)
            {
                await this._connection.CloseAsync();
            }
        }
    });

    private IResult Validate(Dto dto) =>
        CatchResult(() =>
        {
            Check.MustBeNotNull(dto);
            Check.MustBeNotNull(dto.Name);
            Check.MustBeNotNull(dto.Namespace);
            Check.MustBe(dto.ModuleId is not null and not 0);
        });
}