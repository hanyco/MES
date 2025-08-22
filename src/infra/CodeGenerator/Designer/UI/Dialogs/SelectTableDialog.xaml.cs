using System.Windows;
using System.Windows.Controls;

using CodeGenerator.Application.Settings;
using CodeGenerator.UI;

using DataLib;
using DataLib.Extensions;

using Library.Resulting;

namespace CodeGenerator.Designer.UI.Dialogs;

/// <summary>
/// Interaction logic for SelectTableDialog.xaml
/// </summary>
public partial class SelectTableDialog : Window
{
    public SelectTableDialog() => this.InitializeComponent();

    public Table? SelectedTable { get; private set; }

    public static IResult<Table> Ask()
    {
        var dlg = Create();
        var ok = dlg.ShowDialog();
        return ok is true && dlg.SelectedTable is Table table
                ? Result.Success(table)
                : Result.Fail<Table>("No table selected.");
    }

    public static SelectTableDialog Create()
    {
        var dialog = new SelectTableDialog
        {
            Owner = App.Current.MainWindow,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };
        return dialog;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        this.DialogResult = false;
        this.SelectedTable = null;
        this.Close();
    }

    private void OkButton_Click(object sender, RoutedEventArgs e) =>
        this.ValidateAndlose();

    private void TableTreeView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) =>
        this.ValidateAndlose();

    private void ValidateAndlose()
    {
        if (this.OkButton.IsEnabled && this.TableTreeView.GetSelectedItem<Table>() is Table table)
        {
            this.SelectedTable = table;
            this.DialogResult = true;
            this.Close();
        }
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        this.OkButton.IsEnabled = false;
        var tables = await Database.GetTables(Settings.Default.ConnectionString).ToListAsync();

        // Populate the TreeView with tables
        this.TableTreeView.Items.Clear();
        foreach (var table in tables)
        {
            var schemaItem = this.TableTreeView.Items.FindOrAdd(
                x => x.Tag is string schema && schema == table.Schema,
                () => new TreeViewItem
                {
                    Header = table.Schema,
                    Tag = table.Schema
                });
            _ = schemaItem.Items.Add(new TreeViewItem
            {
                Header = table.Name,
                Tag = table
            });
        }
        this.OkButton.IsEnabled = true;
    }
}