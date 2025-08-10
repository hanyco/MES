namespace CodeGenerator.Application.Services;

public sealed partial class Settings
{
    public static Settings Default { get; internal set; } = default!;
    public string ConnectionString { get; init; } = default!;
    public FolderStructure Folders
    {
        get
        {
            field ??= new();
            return field;
        }
        init;
    }
}

public sealed partial class FolderStructure
{
    public string DefaultRoot
    {
        get;
        set
        {
            field = value;
            this.UpdateOtherFolders(field);
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