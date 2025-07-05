using System.Windows.Controls;

using CodeGenerator.Designer.UI.Dialogs;

namespace CodeGenerator.Designer.UI.Pages;

/// <summary>
/// Interaction logic for DtoManagementPage.xaml
/// </summary>
public partial class DtoManagementPage : UserControl
{
    public DtoManagementPage()
    {
        this.InitializeComponent();
        this.DataContextChanged += this.DtoManagementPage_DataContextChanged;

    }

    private void DtoManagementPage_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        => this.EntityDesignerGrid.IsEnabled = e.NewValue is not null;

    private void NewDtoButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        var result = SelectTableDialog.Ask();
    }
}