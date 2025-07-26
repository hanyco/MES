using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

using CodeGenerator.Designer.UI.Dialogs;
using CodeGenerator.Designer.UI.ViewModels;

using DataLib.SqlServer;

using Microsoft.WindowsAPICodePack.Dialogs;

namespace CodeGenerator.Designer.UI.Pages;

/// <summary>
/// Interaction logic for DtoManagementPage.xaml
/// </summary>
public partial class DtoManagementPage : UserControl
{
    public static readonly DependencyProperty StaticViewModelProperty =
        DependencyProperty.Register("StaticViewModel", typeof(DtoManagementPageStaticViewModel), typeof(DtoManagementPage), new PropertyMetadata(null));

    private readonly IDtoService _dtoService;
    private readonly IModuleService _moduleService;

    public DtoManagementPage(IDtoService dtoService, IModuleService moduleService)
    {
        this._dtoService = dtoService;
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

    private async void DtoManagementPage_Loaded(object sender, RoutedEventArgs e)
    {
        this.Loaded -= this.DtoManagementPage_Loaded;
        await this.LoadStaticViewModelAsync();
    }

    private void GenerateCodeButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (this.DataContext is not DtoViewModel vm)
            {
                return;
            }

            var entity = vm
                .ToEntity()
                .With(x => x
                    .Properties = [.. x.Properties.Select(p => p.With(x => x.TypeFullName = SqlTypeUtils.ToNetTypeName(p.TypeFullName ?? string.Empty)))]);
            var codeGenerationResult = this._dtoService.GenerateCodes(entity).ThrowOnFail(this, "Error occurred on generating code.");
            this.CodesViewer.Codes = codeGenerationResult.Value;
        }
        catch (Exception ex)
        {
            TaskDialog.Error(ex.GetBaseException().Message);
        }
    }

    private async Task LoadStaticViewModelAsync()
    {
        var modules = await this._moduleService.GetAll().ParseValue().ToViewModel();
        var dataTypes = SqlTypeUtils.GetSqlTypes().Select(x => x.SqlTypeName);
        this.StaticViewModel = new(modules, dataTypes);
    }

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

    private async void SaveToDbButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (this.DataContext is not DtoViewModel vm)
            {
                return;
            }

            var dto = vm.ToEntity();
            if (dto is null)
            {
                return;
            }

            if (vm.Id is null or 0)
            {
                var id = await this._dtoService.Insert(dto).ThrowOnFail().ParseValue();
                vm.Id = id;
            }
            else
            {
                _ = await this._dtoService.Update(vm.Id.Value, dto).ThrowOnFail();
            }

            TaskDialog.Info("DTO saved successfully.");
        }
        catch (Exception ex)
        {
            TaskDialog.Error(ex);
        }
    }
}

public sealed partial class DtoManagementPageStaticViewModel(IEnumerable<ModuleViewModel> modules, IEnumerable<string> dataTypes)
{
    public ObservableCollection<string> DataTypes { get; } = new(dataTypes);
    public ObservableCollection<ModuleViewModel> Modules { get; } = new(modules);
}