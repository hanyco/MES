using CodeGenerator.Application.Domain;

using Dapper;

using DataLib.Extensions;

using Library.Resulting;

using Microsoft.Data.SqlClient;

using Action = Library.Extensions.DelegateExtension;

namespace CodeGenerator.Application.Services;

public partial class DtoService(SqlConnection db)
{
    private readonly SqlConnection _db = db;

    public Task<IResult> Delete(long id, CancellationToken ct = default) => Action.ToResult(() => this._db.ExecuteInTransaction(async () =>
    {
        _ = await this._db.ExecuteAsync("DELETE FROM [infra].[Property] WHERE DtoId = @Id", new { id }).ThrowIfCancellationRequested(ct);
        _ = await this._db.ExecuteAsync("DELETE FROM [infra].[Dto] WHERE Id = @Id", new { Id = id }).ThrowIfCancellationRequested(ct);
    }, ct));

    public Task<IResult<IEnumerable<Dto>>> GetAll(CancellationToken ct = default)
    {
        const string sql = "SELECT * FROM [infra].[Dto]";
        return Action.ToResult(() => this._db.QueryAsync<Dto>(sql));
    }

    public Task<IResult<Dto?>> GetById(long id, CancellationToken ct = default) => Action.ToResult(async () =>
    {
        var dto = await this._db.QueryFirstOrDefaultAsync<Dto>("SELECT * FROM [infra].[Dto] WHERE Id = @Id", new { Id = id }).ThrowIfCancellationRequested(ct);
        if (dto is null)
        {
            return null;
        }
        var props = await this._db.QueryAsync<Property>("SELECT * FROM [infra].[Property] WHERE DtoId = @Id", new { dto.Id }).ThrowIfCancellationRequested(ct);
        dto.Properties.Clear();
        dto.Properties.AddRange(props);
        return dto;
    });

    public Task<IResult<long>> Insert(Dto dto, CancellationToken ct = default) => Action.ToResult(() => this._db.ExecuteInTransaction(async () =>
    {
        const string sql = """
                INSERT INTO [infra].[Dto] (Name, NameSpace, ModuleId, DbObjectId, Guid, Comment, IsParamsDto, IsResultDto, IsViewModel, IsList, BaseType)
                  VALUES (@Name, @Namespace, @ModuleId, @DbObjectId, @Guid, @Comment, @IsParamsDto, @IsResultDto, @IsViewModel, @IsList, @BaseType);
                SELECT CAST(SCOPE_IDENTITY() AS bigint);
                """;
        var id = await this._db.ExecuteScalarAsync<long>(sql, dto).ThrowIfCancellationRequested(ct);
        dto.Id = id;
        await this.InsertProperties(dto, ct);
        return id;
    }, ct));

    public Task<IResult> Update(long id, Dto dto, CancellationToken ct = default) => Action.ToResult(() => this._db.ExecuteInTransaction(async () =>
    {
        const string sql = @"UPDATE [infra].[Dto]
            SET Name = @Name,
                NameSpace = @Namespace,
                ModuleId = @ModuleId,
                DbObjectId = @DbObjectId,
                Comment = @Comment,
                IsParamsDto = @IsParamsDto,
                IsResultDto = @IsResultDto,
                IsViewModel = @IsViewModel,
                IsList = @IsList,
                BaseType = @BaseType
            WHERE Id = @Id";
        _ = await this._db.ExecuteAsync(sql, dto).ThrowIfCancellationRequested(ct);
        _ = await this._db.ExecuteAsync("DELETE FROM [infra].[Property] WHERE DtoId = @Id", new { dto.Id }).ThrowIfCancellationRequested(ct);
        await this.InsertProperties(dto, ct);
    }, ct));

    private async Task InsertProperties(Dto dto, CancellationToken ct)
    {
        const string sql = """
            INSERT INTO [infra].[Property] (ParentEntityId, PropertyType, TypeFullName, Name, HasSetter, HasGetter, IsList, IsNullable, Comment, DbObjectId, Guid, DtoId)
              VALUES (@ParentEntityId, @PropertyType, @TypeFullName, @Name, @HasSetter, @HasGetter, @IsList, @IsNullable, @Comment, @DbObjectId, @Guid, @DtoId)
            """;
        foreach (var field in dto.Properties)
        {
            var param = new
            {
                ParentEntityId = dto.Id,
                field.PropertyType,
                field.TypeFullName,
                field.Name,
                field.HasSetter,
                field.HasGetter,
                field.IsList,
                field.IsNullable,
                field.Comment,
                field.DbObjectId,
                field.Guid,
                DtoId = dto.Id
            };
            _ = await this._db.ExecuteAsync(sql, param).ThrowIfCancellationRequested(ct);
        }
    }
}