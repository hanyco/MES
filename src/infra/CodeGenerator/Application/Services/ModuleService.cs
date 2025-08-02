using System.Diagnostics.CodeAnalysis;

using CodeGenerator.Application.Domain;
using CodeGenerator.Designer.UI.ViewModels;

using Dapper;

using DataLib.Extensions;

using Library.Resulting;

using Microsoft.Data.SqlClient;


namespace CodeGenerator.Application.Services;

internal sealed class ModuleService(SqlConnection connection) : IModuleService
{
    [return: NotNull]
    public Task<IResult<IEnumerable<Module>>> GetAll(CancellationToken ct = default) =>
        CatchResultAsync(() => connection.QueryAsync<Module>("SELECT * FROM [infra].[Module]"));

    [return: NotNull]
    public Task<IResult<Module?>> GetById(long id, CancellationToken ct = default) =>
        CatchResultAsync(() => connection.QueryFirstOrDefaultAsync<Module>("SELECT * FROM [infra].[Module] WHERE Id = @Id", new { Id = id }));
}