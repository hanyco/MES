using System.Diagnostics.CodeAnalysis;

using CodeGenerator.Application.Domain;

using Dapper;

using DataLib.Extensions;

using Library.Coding;
using Library.Resulting;

using Microsoft.Data.SqlClient;

namespace CodeGenerator.Application.Services;

internal sealed partial class PropertyService(SqlConnection connection) : IPropertyService, IDisposable
{
    private readonly SqlConnection _connection = connection;

    [return: NotNull]
    public Task<IResult> Delete(long id, CancellationToken ct = default) => CatchResultAsync(async () =>
    {
        _ = await this._connection.ExecuteAsync("DELETE FROM [infra].[Property] WHERE Id = @Id", new { Id = id });
    });

    [return: NotNull]
    public Task<IResult> DeleteByDtoId(long dtoId, CancellationToken ct = default) => CatchResultAsync(async () =>
    {
        _ = await this._connection.ExecuteAsync("DELETE FROM [infra].[Property] WHERE DtoId = @DtoId", new { DtoId = dtoId });
    });

    [return: NotNull]
    public Task<IResult<IEnumerable<Property>>> GetAll(CancellationToken ct = default) => CatchResultAsync(() =>
        this._connection.QueryAsync<Property>("SELECT * FROM [infra].[Property]"));

    [return: NotNull]
    public Task<IResult<Property?>> GetById(long id, CancellationToken ct = default) => CatchResultAsync(() =>
        this._connection.QueryFirstOrDefaultAsync<Property>("SELECT * FROM [infra].[Property] WHERE Id = @Id", new { Id = id }));

    [return: NotNull]
    public Task<IResult<long>> Insert(Property dto, CancellationToken ct = default) => CatchResultAsync(async () =>
    {
        const string sql = """
        INSERT INTO [infra].[Property]
          (ParentEntityId, PropertyType, TypeFullName, Name, HasSetter, HasGetter, IsList, IsNullable, Comment, DbObjectId, DtoId)
          VALUES (@ParentEntityId, @PropertyType, @TypeFullName, @Name, @HasSetter, @HasGetter, @IsList, @IsNullable, @Comment, @DbObjectId, @ParentEntityId);
        SELECT CAST(SCOPE_IDENTITY() AS bigint);
        """;

        return await this._connection.ExecuteScalarAsync<long>(sql, new
        {
            dto.ParentEntityId,
            dto.PropertyType,
            dto.TypeFullName,
            dto.Name,
            dto.HasSetter,
            dto.HasGetter,
            dto.IsList,
            dto.IsNullable,
            dto.Comment,
            dto.DbObjectId,
        });
    });

    [return: NotNull]
    public Task<IResult> Update(long id, Property dto, CancellationToken ct = default) => CatchResultAsync(async () =>
    {
        const string sql = """
        UPDATE [infra].[Property] SET
          ParentEntityId = @ParentEntityId,
          PropertyType = @PropertyType,
          TypeFullName = @TypeFullName,
          Name = @Name,
          HasSetter = @HasSetter,
          HasGetter = @HasGetter,
          IsList = @IsList,
          IsNullable = @IsNullable,
          Comment = @Comment,
          DbObjectId = @DbObjectId,
          DtoId = @ParentEntityId
        WHERE Id = @Id;
        """;

        _ = await this._connection.ExecuteAsync(sql, new
        {
            dto.ParentEntityId,
            dto.PropertyType,
            dto.TypeFullName,
            dto.Name,
            dto.HasSetter,
            dto.HasGetter,
            dto.IsList,
            dto.IsNullable,
            dto.Comment,
            dto.DbObjectId,
            Id = id,
        });
    });

    public void Dispose() =>
        this._connection.Dispose();
}
