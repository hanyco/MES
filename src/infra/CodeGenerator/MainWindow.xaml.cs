using System.Windows;
using CodeGenerator.UI;

using Microsoft.WindowsAPICodePack.Dialogs;

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
        MainContent.Content = new CodeGenerator.Designer.UI.Pages.DtoManagementPage();
    }

    private void ManageCqrsQueries_Click(object sender, RoutedEventArgs e)
    {
        TaskDialog.Info("Manage CQRS Queries selected");
    }

    private void ManageCqrsCommands_Click(object sender, RoutedEventArgs e)
    {
        TaskDialog.Info("Manage CQRS Commands selected");
    }

    private void BlazorComponents_Click(object sender, RoutedEventArgs e)
    {
        TaskDialog.Info("Blazor Components selected");
    }

    private void BlazorPages_Click(object sender, RoutedEventArgs e)
    {
        TaskDialog.Info("Blazor Pages selected");
    }

    private void Settings_Click(object sender, RoutedEventArgs e)
    {
        TaskDialog.Info("Settings selected");
    }

    private void About_Click(object sender, RoutedEventArgs e)
    {
        TaskDialog.Info("About selected");
    }
}
