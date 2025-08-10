using System.IO;
using System.Text.Json;

namespace CodeGenerator.Application.Services;

public static class SettingsExtension
{
    private const string _FileName = "settings.json";

    extension(Settings)
    {
        public static void Load() =>
            Settings.Configure(GetSettings());

        public static async Task SaveAsync()
        {
            var json = JsonSerializer.Serialize(Settings.Default);
            var path = Path.Combine(AppContext.BaseDirectory, _FileName);
            await File.WriteAllTextAsync(path, json);
        }

        public static void Configure(string connectionString, FolderStructure? folders = null)
            => Configure(new Settings { ConnectionString = connectionString, Folders = folders ?? GetSettings().Folders });

        public static void Configure(Settings settings)
            => Settings.Default = settings;

        public static void Reset()
            => Settings.Default = new Settings { Folders = new FolderStructure() };
    }

    extension(FolderStructure @this)
    {
        public void UpdateOtherFolders(string value)
        {
            if (@this.PagesPath.IsNullOrEmpty())
            {
                @this.PagesPath = Path.Combine(value, "Presentation", "Pages");
            }

            if (@this.ComponentsPath.IsNullOrEmpty())
            {
                @this.ComponentsPath = Path.Combine(value, "Presentation", "Components");
            }

            if (@this.ViewModelsPath.IsNullOrEmpty())
            {
                @this.ViewModelsPath = Path.Combine(value, "Presentation", "ViewModels");
            }

            if (@this.ControllersPath.IsNullOrEmpty())
            {
                @this.ControllersPath = Path.Combine(value, "Presentation", "Controllers");
            }

            if (@this.ApplicationPath.IsNullOrEmpty())
            {
                @this.ApplicationPath = Path.Combine(value, "Application");
            }

            if (@this.ApplicationDtosPath.IsNullOrEmpty())
            {
                @this.ApplicationDtosPath = Path.Combine(@this.ApplicationPath!, "Models");
            }

            if (@this.RepositoriesPath.IsNullOrEmpty())
            {
                @this.RepositoriesPath = Path.Combine(value, "Infrastructure", "Repositories");
            }
        }
    }

    private static Settings GetSettings()
    {
        var path = Path.Combine(AppContext.BaseDirectory, _FileName);
        var settings = File.Exists(path)
            ? JsonSerializer.Deserialize<Settings>(File.ReadAllText(path))!
            : new Settings();
        return settings;
    }
}
