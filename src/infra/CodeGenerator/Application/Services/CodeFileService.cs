using System.IO;
using Library.CodeGenLib.Models;

namespace CodeGenerator.Application.Services;

public static partial class CodeFileService
{
    public static void SaveToDisk(IEnumerable<(Code Code, ProjectLayer Layer)> codes)
    {
        if (codes is null)
        {
            return;
        }

        foreach (var (code, layer) in codes)
        {
            var folder = GetPath(layer);
            if (string.IsNullOrWhiteSpace(folder))
            {
                continue;
            }

            Directory.CreateDirectory(folder);
            var path = Path.Combine(folder, code.FileName);
            File.WriteAllText(path, code.Statement);
        }
    }

    public static string? GetPath(ProjectLayer layer)
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
            _ => root,
        };

        if (string.IsNullOrWhiteSpace(relative))
        {
            return null;
        }

        return Path.IsPathRooted(relative) ? relative : Path.Combine(root, relative);
    }
}
