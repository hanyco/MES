namespace CodeGenerator.Models;
public sealed class Settings
{
    public FolderStructure? Folders { get; set; }
}

public sealed class FolderStructure
{
    public string? DefaultRoot { get; set; }
    public string? PagesPath { get; set; }
    public string? ComponentsPath { get; set; }
    public string? ViewModelsPath { get; set; }
    public string? ControllersPath { get; set; }
    public string? ApplicationPath { get; set; }
    public string? ApplicationModelsPath { get; set; }
    public string? RepositoriesPath { get; set; }
}

public enum ProjectLayer
{
    None,
    Page,
    Component,
    ViewModel,
    Controller,
    Application,
    ApplicationModel,
    Repository,
}

public static class SettingsExtensions
{
    private static Dictionary<ProjectLayer, string> _projectLayerPaths { get; } = [];

    extension(ProjectLayer)
    {
        public static IEnumerable<(ProjectLayer Layer, string Path)> ProjectLayerPaths => _projectLayerPaths.Select(x => (x.Key, x.Value));
    }
}