using System.Windows.Controls;

using CodeGenerator.Application.Services;
using CodeGenerator.Designer.UI.Dialogs;
using CodeGenerator.Designer.UI.ViewModels;

using Library.Coding;

namespace CodeGenerator.Designer.UI.Pages;

/// <summary>
/// Interaction logic for DtoManagementPage.xaml
/// </summary>
public partial class DtoManagementPage : UserControl
{
    private readonly IModuleService _moduleService;

    public DtoManagementPage(IModuleService moduleService)
    {
        this.InitializeComponent();

        this.DataContextChanged += this.DtoManagementPage_DataContextChanged;
        this._moduleService = moduleService;
    }

    private void DtoManagementPage_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        => this.EntityDesignerGrid.IsEnabled = e.NewValue is not null;

    private void NewDtoButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        var response = SelectTableDialog.Ask();
        if (response.IsFailure)
        {
            return;
        }
        var model = DtoManagementPageViewModel.CrateByTable(response.GetValue())
            .With(async x =>
                x.Modules = await this._moduleService
                    .GetAll().ThrowOnFail()
                    .GetValue()
                    .ToViewModel());

        this.DataContext = model;
    }
}