using System.Windows;
using CodeGenerator.Designer.UI.Pages;
using CodeGenerator.Designer.UI.Controls;
using Library.CodeGenLib;
using Library.CodeGenLib.Back;
using Library.CodeGenLib.CodeGenerators;

using Library.Validations;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using CodeGenerator.Application.Settings;

namespace CodeGenerator;

public partial class App : System.Windows.Application
{
    private IConfiguration _configuration = default!;
    private ResourceDictionary _currentTheme = default!;
    private IHost _host = default!;

    public App() =>
        this.Instance = this;

    public App Instance { get; }

    public void UseDarkTheme() =>
        this.ApplyTheme("DarkTheme.xaml");

    public void UseLightTheme() =>
        this.ApplyTheme("LightTheme.xaml");

    protected override async void OnExit(ExitEventArgs e)
    {
        using (this._host)
        {
            await this._host!.StopAsync(CancellationToken.None);
        }
        base.OnExit(e);
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        await this.SetupConfiguration();
        await this.SetupServices();
        this.SetupLayout();
        this.ShowMainWindow();
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
    }

    private async Task SetupConfiguration()
    {
        var builder = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        this._configuration = builder.Build();
        await Settings.Load();
        var connection = this._configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(Settings.Default.ConnectionString) && !string.IsNullOrWhiteSpace(connection))
        {
            Settings.Configure(connection);
            Settings.Save();
        }
    }

    private void SetupLayout()
    {
        var theme = this._configuration["Theme"];
        if (!string.IsNullOrEmpty(theme) && theme.Equals("dark", StringComparison.OrdinalIgnoreCase))
        {
            this.UseDarkTheme();
        }
        else
        {
            this.UseLightTheme();
        }
    }

    private async Task SetupServices()
    {
        this._host = Host.CreateDefaultBuilder()
                    .ConfigureServices((context, services) =>
                    {
                        // Register services
                        _ = services.AddSingleton<MainWindow>()
                            .AddTransient<DtoManagementPage>()
                            .AddTransient<SettingsControl>();

                        _ = services.AddTransient<IPropertyService, PropertyService>();
                        _ = services.AddTransient<IDtoService, DtoService>();
                        _ = services.AddTransient<ICodeGeneratorEngine<INamespace>, RoslynCodeGenerator>();

                        _ = services.AddTransient(x => new SqlConnection(Settings.Default.ConnectionString));

                        _ = services.AddTransient<IModuleService, ModuleService>();
                    })
                    .Build();
        await this._host.StartAsync();
    }

    private void ShowMainWindow()
    {
        var mainWindow = this._host!.Services.GetService<MainWindow>()!;
        mainWindow.Show();
    }
}