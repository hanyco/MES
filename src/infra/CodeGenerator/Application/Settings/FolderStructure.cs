namespace CodeGenerator.Application.Settings;

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
