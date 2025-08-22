namespace CodeGenerator.Application.Settings;

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

    public CodeConfigs CodeConfigs
    {
        get
        {
            field ??= new();
            return field;
        }
        init;
    }
}
