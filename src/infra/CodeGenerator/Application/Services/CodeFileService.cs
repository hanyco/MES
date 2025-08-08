using System.IO;

using Library.CodeGenLib.Models;
using Library.Exceptions;
using Library.Resulting;

namespace CodeGenerator.Application.Services;

public static partial class CodeFileService
{
    public static IResult<string> SaveToDisk(IEnumerable<(Code Code, ProjectLayer Layer)> codes)
    {
        if (codes is null)
        {
            return Result.Fail<string>(new ValidationException($"{nameof(codes)} cannot be null"));
        }

        foreach (var (code, layer) in codes)
        {
            var folder = GetPath(layer);
            if (string.IsNullOrWhiteSpace(folder))
            {
                continue;
            }

            _ = Directory.CreateDirectory(folder);
            var path = Path.Combine(folder, code.FileName);
            File.WriteAllText(path, code.Statement);
        }
        return Result.Success(GetPath(ProjectLayer.None)!);
    }

    private static string? GetPath(ProjectLayer layer)
    {
        var folders = Settings.Default.Folders;
        var root = folders.DefaultRoot;

        var relative = layer switch
        {
            ProjectLayer.Page => folders.PagesPath,
            ProjectLayer.Component => folders.ComponentsPath,
            ProjectLayer.ViewModel => folders.ViewModelsPath,
            ProjectLayer.Controller => folders.ControllersPath,
            ProjectLayer.Application => folders.ApplicationPath,
            ProjectLayer.ApplicationModel => folders.ApplicationDtosPath,
            ProjectLayer.Repository => folders.RepositoriesPath,
            ProjectLayer.None => root,
            _ => root,
        };

        return string.IsNullOrWhiteSpace(relative) ? null : Path.IsPathRooted(relative) ? relative : Path.Combine(root, relative);
    }
}
