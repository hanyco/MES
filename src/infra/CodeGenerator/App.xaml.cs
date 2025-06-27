using System.Windows;

using CodeGenerator.Designer.UI.ViewModels;

namespace CodeGenerator;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : System.Windows.Application
{

    protected override void OnStartup(StartupEventArgs e)
    {
        var shell = new ShellViewModel();

        var wnd = new MainWindow { DataContext = shell };
        wnd.Show();
    }
}