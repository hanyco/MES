using System.Diagnostics.CodeAnalysis;

using CodeGenerator.Designer.UI.ViewModels;

using DataLib;

namespace CodeGenerator.Application.Domain;

internal static class Extensions
{
    extension(Property)
    {
        [return: NotNull]
        public static Property GetByTableField([DisallowNull]Field tableField) => new()
        {
            DbObjectId = tableField.ObjectId.ToString(),
            Comment = tableField.Comment,
            IsNullable = tableField.AllowNull,
            Name = tableField.Name,
            TypeFullName = tableField.Type
        };
    }

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