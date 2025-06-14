using System.Data;
using Dapper;
using CodeGenerator.Application.Domain;

namespace CodeGenerator.Application.Services;

public partial class DtoService(IDbConnection db)
{
    private readonly IDbConnection _db = db;

    public Task<int> Delete(long id, CancellationToken ct = default)
        => this._db.ExecuteAsync("DELETE FROM [infra].[Dto] WHERE Id = @Id", new { Id = id });

    public async Task<IEnumerable<Dto>> GetAll(CancellationToken ct = default)
    {
        const string sql = "SELECT * FROM [infra].[Dto]";
        var dtos = (await _db.QueryAsync<Dto>(sql)).ToList();
        foreach (var dto in dtos)
        {
            var props = await _db.QueryAsync<Property>(
                "SELECT * FROM [infra].[Property] WHERE DtoId = @Id",
                new { dto.Id });
            dto.Properties.Clear();
            dto.Properties.AddRange(props);
        }
        return dtos;
    }

    public async Task<Dto?> GetById(long id, CancellationToken ct = default)
    {
        const string sql = "SELECT * FROM [infra].[Dto] WHERE Id = @Id";
        var dto = await this._db.QueryFirstOrDefaultAsync<Dto>(sql, new { Id = id });
        if (dto is null)
        {
            return null;
        }
        var props = await this._db.QueryAsync<Property>(
            "SELECT * FROM [infra].[Property] WHERE DtoId = @Id",
            new { dto.Id });
        dto.Properties.Clear();
        dto.Properties.AddRange(props);
        return dto;
    }

    public async Task<long> Insert(Dto dto, CancellationToken ct = default)
    {
        const string sql = @"INSERT INTO [infra].[Dto]
            (Name, NameSpace, ModuleId, DbObjectId, Guid, Comment, IsParamsDto,
             IsResultDto, IsViewModel, IsList, BaseType)
            VALUES (@Name, @Namespace, @ModuleId, @DbObjectId, @Guid, @Comment,
                    @IsParamsDto, @IsResultDto, @IsViewModel, @IsList, @BaseType);
            SELECT CAST(SCOPE_IDENTITY() AS bigint);";
        var id = await this._db.ExecuteScalarAsync<long>(sql, dto);
        dto.Id = id;
        await this.SaveProperties(dto, ct);
        return id;
    }

    public async Task Update(Dto dto, CancellationToken ct = default)
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
        await _db.ExecuteAsync(sql, dto);
        await _db.ExecuteAsync("DELETE FROM [infra].[Property] WHERE DtoId = @Id", new { dto.Id });
        await SaveProperties(dto, ct);
    }

    public Task<int> Delete(long id, CancellationToken ct = default)
        _ = await this._db.ExecuteAsync(sql, dto);
        _ = await this._db.ExecuteAsync("DELETE FROM [infra].[Property] WHERE DtoId = @Id", new { dto.Id });
        await this.SaveProperties(dto, ct);
    }

    private async Task SaveProperties(Dto dto, CancellationToken ct)
    {
        const string sql = @"INSERT INTO [infra].[Property]
            (ParentEntityId, PropertyType, TypeFullName, Name, HasSetter, HasGetter,
             IsList, IsNullable, Comment, DbObjectId, Guid, DtoId)
            VALUES (@ParentEntityId, @PropertyType, @TypeFullName, @Name, @HasSetter,
                    @HasGetter, @IsList, @IsNullable, @Comment, @DbObjectId, @Guid, @DtoId)";
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
            await this._db.ExecuteAsync(sql, param, ct);
        }
    }
}
