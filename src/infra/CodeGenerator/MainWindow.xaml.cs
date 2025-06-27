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
        MsgBox.ShowInfo("Manage Models selected");
    }

    private void ManageCqrsQueries_Click(object sender, RoutedEventArgs e)
    {
        MsgBox.ShowInfo("Manage CQRS Queries selected");
    }

    private void ManageCqrsCommands_Click(object sender, RoutedEventArgs e)
    {
        MsgBox.ShowInfo("Manage CQRS Commands selected");
    }

    private void BlazorComponents_Click(object sender, RoutedEventArgs e)
    {
        MsgBox.ShowInfo("Blazor Components selected");
    }

    private void BlazorPages_Click(object sender, RoutedEventArgs e)
    {
        MsgBox.ShowInfo("Blazor Pages selected");
    }

    private void Settings_Click(object sender, RoutedEventArgs e)
    {
        MsgBox.ShowInfo("Settings selected");
    }

    private void About_Click(object sender, RoutedEventArgs e)
    {
        MsgBox.ShowInfo("About selected");
    }
}
