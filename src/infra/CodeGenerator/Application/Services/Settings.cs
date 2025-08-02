using System.IO;

namespace CodeGenerator.Application.Services;

public sealed class Settings
{
    public static Settings Default { get; private set; } = default!;
    public string ConnectionString { get; private init; } = default!;
    public FolderStructure? Folders { get; set; }

    public static void Configure(string connectionString)
        => Default = new Settings { ConnectionString = connectionString };
}

public sealed partial class FolderStructure
{
    private string _defaultRoot = default!;

    public string DefaultRoot
    {
        get => _defaultRoot;
        set
        {
            _defaultRoot = value;

            if (string.IsNullOrWhiteSpace(PagesPath))
                PagesPath = Path.Combine(_defaultRoot, "Presentation", "Pages");
            if (string.IsNullOrWhiteSpace(ComponentsPath))
                ComponentsPath = Path.Combine(_defaultRoot, "Presentation", "Components");
            if (string.IsNullOrWhiteSpace(ViewModelsPath))
                ViewModelsPath = Path.Combine(_defaultRoot, "Presentation", "ViewModels");
            if (string.IsNullOrWhiteSpace(ControllersPath))
                ControllersPath = Path.Combine(_defaultRoot, "Presentation", "Controllers");
            if (string.IsNullOrWhiteSpace(ApplicationPath))
                ApplicationPath = Path.Combine(_defaultRoot, "Application");
            if (string.IsNullOrWhiteSpace(ApplicationModelsPath))
                ApplicationModelsPath = Path.Combine(ApplicationPath!, "Models");
            if (string.IsNullOrWhiteSpace(RepositoriesPath))
                RepositoriesPath = Path.Combine(_defaultRoot, "Infrastructure", "Repositories");
        }
    }

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