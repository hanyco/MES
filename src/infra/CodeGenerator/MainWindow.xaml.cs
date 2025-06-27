using System.Windows;

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
        MessageBox.Show("Manage Models selected", "Info");
    }

    private void ManageCqrsQueries_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Manage CQRS Queries selected", "Info");
    }

    private void ManageCqrsCommands_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Manage CQRS Commands selected", "Info");
    }

    private void BlazorComponents_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Blazor Components selected", "Info");
    }

    private void BlazorPages_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Blazor Pages selected", "Info");
    }

    private void Settings_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Settings selected", "Info");
    }

    private void About_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("About selected", "Info");
    }
}
