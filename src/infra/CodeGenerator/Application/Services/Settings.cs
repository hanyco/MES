using System.IO;

namespace CodeGenerator.Application.Services;

public sealed partial class Settings
{
    public static Settings Default { get; private set; } = default!;
    public string ConnectionString { get; private init; } = default!;
    public FolderStructure? Folders { get; set; }

    public static void Configure(string connectionString)
        => Default = new Settings { ConnectionString = connectionString };
}

public sealed partial class FolderStructure
{
    public string DefaultRoot
    {
        get;
        set
        {
            field = value;

            if (this.PagesPath.IsNullOrEmpty())
            {
                this.PagesPath = Path.Combine(field, "Presentation", "Pages");
            }

            if (this.ComponentsPath.IsNullOrEmpty())
            {
                this.ComponentsPath = Path.Combine(field, "Presentation", "Components");
            }

            if (this.ViewModelsPath.IsNullOrEmpty())
            {
                this.ViewModelsPath = Path.Combine(field, "Presentation", "ViewModels");
            }

            if (this.ControllersPath.IsNullOrEmpty())
            {
                this.ControllersPath = Path.Combine(field, "Presentation", "Controllers");
            }

            if (this.ApplicationPath.IsNullOrEmpty())
            {
                this.ApplicationPath = Path.Combine(field, "Application");
            }

            if (this.ApplicationDtosPath.IsNullOrEmpty())
            {
                this.ApplicationDtosPath = Path.Combine(this.ApplicationPath!, "Models");
            }

            if (this.RepositoriesPath.IsNullOrEmpty())
            {
                this.RepositoriesPath = Path.Combine(field, "Infrastructure", "Repositories");
            }
        }
    } = default!;

    public string? PagesPath { get; set; }
    public string? ComponentsPath { get; set; }
    public string? ViewModelsPath { get; set; }
    public string? ControllersPath { get; set; }
    public string? ApplicationPath { get; set; }
    public string? ApplicationDtosPath { get; set; }
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