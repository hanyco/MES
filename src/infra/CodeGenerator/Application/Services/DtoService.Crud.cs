using System.Data;
using Dapper;
using CodeGenerator.Application.Domain;
using Library.CodeGenLib.Models;

namespace CodeGenerator.Application.Services;

public partial class DtoService
{
    private readonly IDbConnection _db;

    public DtoService(IDbConnection db)
    {
        _db = db;
    }

    public async Task<IEnumerable<DtoDefinition>> GetAll(CancellationToken ct = default)
    {
        const string sql = "SELECT * FROM [infra].[Dto]";
        var dtos = (await _db.QueryAsync<DtoDefinition>(sql)).ToList();
        foreach (var dto in dtos)
        {
            var props = await _db.QueryAsync<FieldDefinition>(
                "SELECT Name, TypeFullName AS [Type], Comment FROM [infra].[Property] WHERE DtoId = @Id",
                new { dto.Id });
            dto.Fields.Clear();
            dto.Fields.AddRange(props);
        }
        return dtos;
    }

    public async Task<DtoDefinition?> GetById(long id, CancellationToken ct = default)
    {
        const string sql = "SELECT * FROM [infra].[Dto] WHERE Id = @Id";
        var dto = await _db.QueryFirstOrDefaultAsync<DtoDefinition>(sql, new { Id = id });
        if (dto is null)
        {
            return null;
        }
        var props = await _db.QueryAsync<FieldDefinition>(
            "SELECT Name, TypeFullName AS [Type], Comment FROM [infra].[Property] WHERE DtoId = @Id",
            new { Id = dto.Id });
        dto.Fields.Clear();
        dto.Fields.AddRange(props);
        return dto;
    }

    public async Task<long> Insert(DtoDefinition dto, CancellationToken ct = default)
    {
        const string sql = @"INSERT INTO [infra].[Dto]
            (Name, NameSpace, ModuleId, DbObjectId, Guid, Comment, IsParamsDto,
             IsResultDto, IsViewModel, IsList, BaseType)
            VALUES (@Name, @Namespace, @ModuleId, @DbObjectId, @Guid, @Comment,
                    @IsParamsDto, @IsResultDto, @IsViewModel, @IsList, @BaseType);
            SELECT CAST(SCOPE_IDENTITY() AS bigint);";
        var id = await _db.ExecuteScalarAsync<long>(sql, dto);
        dto.Id = id;
        await SaveProperties(dto, ct);
        return id;
    }

    public async Task Update(DtoDefinition dto, CancellationToken ct = default)
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
        => _db.ExecuteAsync("DELETE FROM [infra].[Dto] WHERE Id = @Id", new { Id = id });

    private async Task SaveProperties(DtoDefinition dto, CancellationToken ct)
    {
        const string sql = @"INSERT INTO [infra].[Property]
            (ParentEntityId, PropertyType, TypeFullName, Name, HasSetter, HasGetter,
             IsList, IsNullable, Comment, DbObjectId, Guid, DtoId)
            VALUES (@ParentEntityId, @PropertyType, @TypeFullName, @Name, @HasSetter,
                    @HasGetter, @IsList, @IsNullable, @Comment, @DbObjectId, @Guid, @DtoId)";
        foreach (var field in dto.Fields)
        {
            var param = new
            {
                ParentEntityId = dto.Id,
                PropertyType = 0,
                TypeFullName = field.Type,
                Name = field.Name,
                HasSetter = true,
                HasGetter = true,
                IsList = false,
                IsNullable = false,
                Comment = field.Comment,
                DbObjectId = (string?)null,
                Guid = Guid.NewGuid(),
                DtoId = dto.Id
            };
            await _db.ExecuteAsync(sql, param, ct);
        }
    }
}
