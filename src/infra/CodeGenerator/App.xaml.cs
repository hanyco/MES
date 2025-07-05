using System.Windows;

using CodeGenerator.Application.Services;

using Microsoft.Extensions.Configuration;

namespace CodeGenerator;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : System.Windows.Application
{
    private ResourceDictionary? _currentTheme;

    public IConfiguration? Configuration { get; private set; }

    public void UseDarkTheme() => this.ApplyTheme("DarkTheme.xaml");

    public void UseLightTheme() => this.ApplyTheme("LightTheme.xaml");

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Setup configuration using appsettings.json file.
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        this.Configuration = builder.Build();

        Settings.Configure(this.Configuration.GetConnectionString("DefaultConnection")!);

        // Use theme based on configuration "Theme" value.
        var theme = this.Configuration["Theme"];
        if (!string.IsNullOrEmpty(theme) && theme.Equals("dark", StringComparison.OrdinalIgnoreCase))
        {
            this.UseDarkTheme();
        }
        else
        {
            this.UseLightTheme();
        }
    }

    private void ApplyTheme(string themeFile)
    {
        if (this._currentTheme is not null)
        {
            _ = this.Resources.MergedDictionaries.Remove(this._currentTheme);
        }

        var dict = new ResourceDictionary
        {
            Source = new Uri($"Designer/UI/Styles/{themeFile}", UriKind.Relative)
        };

        this.Resources.MergedDictionaries.Insert(0, dict);
        this._currentTheme = dict;
        this.OnThemeApplied(themeFile);
    }

    partial void OnThemeApplied(string themeFile);
}