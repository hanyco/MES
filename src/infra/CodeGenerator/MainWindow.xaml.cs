using System.Windows;
using CodeGenerator.UI;

namespace CodeGenerator;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void ManageModels_Click(object sender, RoutedEventArgs e)
    {
        DialogService.ShowInfo("Manage Models selected", "Info");
    }

    private void ManageCqrsQueries_Click(object sender, RoutedEventArgs e)
    {
        DialogService.ShowInfo("Manage CQRS Queries selected", "Info");
    }

    private void ManageCqrsCommands_Click(object sender, RoutedEventArgs e)
    {
        DialogService.ShowInfo("Manage CQRS Commands selected", "Info");
    }

    private void BlazorComponents_Click(object sender, RoutedEventArgs e)
    {
        DialogService.ShowInfo("Blazor Components selected", "Info");
    }

    private void BlazorPages_Click(object sender, RoutedEventArgs e)
    {
        DialogService.ShowInfo("Blazor Pages selected", "Info");
    }

    private void Settings_Click(object sender, RoutedEventArgs e)
    {
        DialogService.ShowInfo("Settings selected", "Info");
    }

    private void About_Click(object sender, RoutedEventArgs e)
    {
        DialogService.ShowInfo("About selected", "Info");
    }
}
