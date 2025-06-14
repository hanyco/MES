using System.Windows;

using CodeGenerator.Application.Services;
using CodeGenerator.Designer.UI.ViewModels;


using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace CodeGenerator;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : System.Windows.Application
{

    protected override void OnStartup(StartupEventArgs e)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var connStr = config.GetConnectionString("DefaultConnection")!;
        var db = new SqlConnection(connStr);

        var dtoService = new DtoService(db);
        var dtosPage = new DtosPageViewModel(dtoService);
        var shell = new ShellViewModel(dtosPage);

        var wnd = new MainWindow { DataContext = shell };
        wnd.Show();
    }
}