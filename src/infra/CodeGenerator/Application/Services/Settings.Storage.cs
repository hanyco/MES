using System.IO;
using System.Text.Json;

namespace CodeGenerator.Application.Services;

public sealed partial class Settings
{
    private const string FileName = "settings.json";

    public static void Load()
    {
        var path = Path.Combine(AppContext.BaseDirectory, FileName);
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            Default = JsonSerializer.Deserialize<Settings>(json)!;
        }
        else
        {
            Default = new Settings();
        }
        OnLoaded(Default);
    }

    public void Save()
    {
        var path = Path.Combine(AppContext.BaseDirectory, FileName);
        var json = JsonSerializer.Serialize(this);
        File.WriteAllText(path, json);
        this.OnSaved();
    }

    static partial void OnLoaded(Settings settings);
    partial void OnSaved();
}
