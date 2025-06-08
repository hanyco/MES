using System.Windows;

using CodeGenerator.Application.DependencyInjection;
using CodeGenerator.Designer.UI.ViewModels;

using Library.CodeGenLib;
using Library.CodeGenLib.Back;
using Library.CodeGenLib.CodeGenerators;
using CodeGenerator.Application.Services;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeGenerator;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : System.Windows.Application
{
    public IServiceProvider Services { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(config);

        // Only IDbConnection and MediatR
        _ = services.AddApplicationLayer(sp =>
            new SqlConnection(sp.GetRequiredService<IConfiguration>()
                              .GetConnectionString("DefaultConnection")!));

        // CodeGen engine
        _ = services.AddTransient<ICodeGeneratorEngine<INamespace>, RoslynCodeGenerator>();

        // Services
        _ = services.AddSingleton(sp =>
            new DtoService(sp.GetRequiredService<IConfiguration>()
                .GetConnectionString("DefaultConnection") ?? string.Empty));

        // ViewModels
        _ = services.AddTransient<DtosPageViewModel>();
        _ = services.AddTransient<ShellViewModel>();

        // Build & show
        var sp = services.BuildServiceProvider();
        var wnd = new MainWindow { DataContext = sp.GetRequiredService<ShellViewModel>() };
        wnd.Show();
    }
}