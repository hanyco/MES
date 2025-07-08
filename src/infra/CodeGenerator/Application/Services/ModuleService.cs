using System.Diagnostics.CodeAnalysis;

using CodeGenerator.Application.Domain;
using CodeGenerator.Designer.UI.ViewModels;

using Dapper;

using DataLib.Extensions;

using Library.Resulting;

using Microsoft.Data.SqlClient;

using Action = Library.Extensions.DelegateExtension;

namespace CodeGenerator.Application.Services;

internal sealed class ModuleService(SqlConnection connection) : IModuleService
{
    [return: NotNull]
    public Task<IResult<IEnumerable<Module>>> GetAll(CancellationToken ct = default) =>
        Action.ToResult(() => connection.QueryAsync<Module>("SELECT * FROM [infra].[Module]"));

    [return: NotNull]
    public Task<IResult<Module?>> GetById(long id, CancellationToken ct = default) =>
        Action.ToResult(() => connection.QueryFirstOrDefaultAsync<Module>("SELECT * FROM [infra].[Module] WHERE Id = @Id", new { Id = id }));
}

public static class ModelConverter
{
    [return: NotNullIfNotNull(nameof(model))]
    public static ModuleViewModel ToViewModel(this Module model) => new()
    {
        Id = model.Id,
        Name = model.Name,
    };

    [return: NotNull]
    public static IEnumerable<ModuleViewModel> ToViewModel([AllowNull] this IEnumerable<Module> models)
        => models?.Select(ToViewModel) ?? [];

    public static async Task<IEnumerable<ModuleViewModel>> ToViewModel(this Task<IEnumerable<Module>> task)
    {
        var value = await task;
        return value.ToViewModel();
    }
}