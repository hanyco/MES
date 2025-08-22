using System.Diagnostics.CodeAnalysis;

using CodeGenerator.Designer.UI.ViewModels;

using DataLib;

namespace CodeGenerator.Application.Domain;

internal static class Extensions
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

    [return: NotNullIfNotNull(nameof(model))]
    public static DtoViewModel ToViewModel(this Dto model) => new()
    {
        Id = model.Id,
        Name = model.Name,
    };

    [return: NotNull]
    public static IEnumerable<DtoViewModel> ToViewModel([AllowNull] this IEnumerable<Dto> models)
        => models?.Select(ToViewModel) ?? [];

    public static async Task<IEnumerable<DtoViewModel>> ToViewModel(this Task<IEnumerable<Dto>> task)
    {
        var value = await task;
        return value.ToViewModel();
    }
}
