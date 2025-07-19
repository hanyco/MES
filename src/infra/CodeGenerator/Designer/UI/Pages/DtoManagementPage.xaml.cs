using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

using CodeGenerator.Application.Services;
using CodeGenerator.Designer.UI.Dialogs;
using CodeGenerator.Designer.UI.ViewModels;
using DataLib.SqlServer;
using System.Linq;

namespace CodeGenerator.Designer.UI.Pages;

/// <summary>
/// Interaction logic for DtoManagementPage.xaml
/// </summary>
public partial class DtoManagementPage : UserControl
{
    public static readonly DependencyProperty StaticViewModelProperty =
        DependencyProperty.Register("StaticViewModel", typeof(DtoManagementPageStaticViewModel), typeof(DtoManagementPage), new PropertyMetadata(null));

    private readonly IModuleService _moduleService;

    public DtoManagementPage(IModuleService moduleService)
    {
        this._moduleService = moduleService;

        this.InitializeComponent();

        this.DataContextChanged += this.DtoManagementPage_DataContextChanged;
        this.Loaded += this.DtoManagementPage_Loaded;
    }

    public DtoManagementPageStaticViewModel StaticViewModel
    {
        get => (DtoManagementPageStaticViewModel)this.GetValue(StaticViewModelProperty);
        set => this.SetValue(StaticViewModelProperty, value);
    }

    private void DtoManagementPage_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) =>
        this.EntityDesignerGrid.IsEnabled = e.NewValue is not null;

    private void NewDtoButton_Click(object sender, RoutedEventArgs e)
    {
        var (isOk, value) = SelectTableDialog.Ask();
        if (!isOk)
        {
            return;
        }
        var model = DtoViewModel.CrateByTable(value);

        this.DataContext = model;
    }

    private async Task LoadStaticViewModelAsync()
    {
        var modules = await this._moduleService.GetAll().ParseValue().ToViewModel();
        var dataTypes = SqlTypeUtils.GetSqlTypes().Select(x => x.SqlTypeName);
        this.StaticViewModel = new(modules, dataTypes);
    }

    private async void DtoManagementPage_Loaded(object sender, RoutedEventArgs e)
    {
        this.Loaded -= this.DtoManagementPage_Loaded;
        await this.LoadStaticViewModelAsync();
    }
}

public sealed partial class DtoManagementPageStaticViewModel(IEnumerable<ModuleViewModel> modules, IEnumerable<string> dataTypes)
{
    public ObservableCollection<ModuleViewModel> Modules { get; } = new(modules);

    public ObservableCollection<string> DataTypes { get; } = new(dataTypes);
}