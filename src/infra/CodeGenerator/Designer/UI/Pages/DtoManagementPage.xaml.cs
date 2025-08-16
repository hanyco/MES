using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

using CodeGenerator.Designer.UI.Dialogs;
using CodeGenerator.Designer.UI.ViewModels;
using CodeGenerator.UI.Dialogs;

using DataLib.Extensions;
using DataLib.SqlServer;

using Library.CodeGenLib.Models;
using Library.Coding;
using Library.Exceptions;

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
        this.UpdateUiEnabledState();
    }

    public DtoManagementPageStaticViewModel StaticViewModel
    {
        get => (DtoManagementPageStaticViewModel)this.GetValue(StaticViewModelProperty);
        set => this.SetValue(StaticViewModelProperty, value);
    }

    private void DtoManagementPage_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) =>
        this.UpdateUiEnabledState();

    private void UpdateUiEnabledState()
    {
        var isEnabled = this.DataContext is DtoViewModel;
        this.SaveGeneratedCodesToDiskutton.IsEnabled = isEnabled;
        this.EntityDesignerGrid.IsEnabled = isEnabled;
        this.GenerateCodeButton.IsEnabled = isEnabled;
        this.SaveToDbButton.IsEnabled = isEnabled;
        this.CodesViewer.IsEnabled = isEnabled;

        this.DtosListBox.IsEnabled = !isEnabled;
        this.NewDtoButton.IsEnabled = !isEnabled;
        this.EditDtoButton.IsEnabled = !isEnabled;
        this.DeleteDtoButton.IsEnabled = !isEnabled;
    }

    private async void DtoManagementPage_Loaded(object sender, RoutedEventArgs e)
    {
        this.Loaded -= this.DtoManagementPage_Loaded;
        await this.LoadStaticViewModelAsync();
    }

    private void GenerateCodeButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            this.CodesViewer.Codes = this.GenerateCode();
        }
        catch (Exception ex)
        {
            TaskDialog.Error(ex.GetBaseException().Message);
        }
    }

    private Codes GenerateCode()
    {
        if (this.DataContext is not DtoViewModel vm)
        {
            throw new Exception("No DTO is created yet.");
        }

        static Property setTypeName(Property p) => p.With(x => x.TypeFullName = SqlType.ToNetType(p.TypeFullName!).FullName);
        var entity = vm.ToEntity().With(x => x.Properties = [.. x.Properties.Select(setTypeName)]);
        var codeGenerationResult = this._dtoService.GenerateCodes(entity).ThrowOnFail();
        return codeGenerationResult.Value;
    }

    private async Task LoadStaticViewModelAsync()
    {
        var modules = await this._moduleService.GetAll().ParseValue().ToViewModel();
        var dataTypes = SqlType.GetSqlTypes().Select(x => x.SqlType.SqlTypeName);
        var dtos = await this._dtoService.GetAll().ParseValue().ToViewModel();
        this.StaticViewModel = new(modules, dataTypes, dtos);
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

    private async void EditDtoButton_Click(object sender, RoutedEventArgs e)
    {
        if (this.DtosListBox.SelectedItem is not DtoViewModel selected || selected.Id is null)
        {
            TaskDialog.Info("Select a DTO first.");
            return;
        }

        try
        {
            var dto = await this._dtoService.GetById(selected.Id.Value).ThrowOnFail().ParseValue();
            if (dto is null)
            {
                TaskDialog.Error("DTO not found.");
                return;
            }

            this.DataContext = this.MapDtoToViewModel(dto);
        }
        catch (Exception ex)
        {
            TaskDialog.Error(ex);
        }
    }

    private async void DeleteDtoButton_Click(object sender, RoutedEventArgs e)
    {
        if (this.DtosListBox.SelectedItem is not DtoViewModel selected || selected.Id is null)
        {
            TaskDialog.Warning("Select a DTO first.");
            return;
        }

        var resp = TaskDialog.AskWithWarning($"Delete DTO '{selected.Name}'?", "Delete Confirmation");
        if (resp != TaskDialogResult.Yes)
        {
            return;
        }

        try
        {
            _ = await this._dtoService.Delete(selected.Id.Value).ThrowOnFail();
            _ = this.StaticViewModel?.Dtos.Remove(selected);
            TaskDialog.Info("DTO deleted successfully.");
        }
        catch (Exception ex)
        {
            TaskDialog.Error(ex);
        }
    }

    private async void SaveToDbButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await this.SaveMetaDataToDb();
        }
        catch (Exception ex)
        {
            TaskDialog.Error(ex);
        }
    }

    private async Task SaveMetaDataToDb()
    {
        if (this.DataContext is not DtoViewModel vm)
        {
            throw new ValidationException("No view model found.");
        }

        var dto = vm.ToEntity();
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

    private void SaveGeneratedCodesToDisk_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var path = this.SaveGeneratedCodesToDisk();
            _ = TaskDialog.Create()
            .WithIcon(TaskDialogStandardIcon.Information)
            .WithInstructionText("Codes are saved.")
            .WithCaption("Save code")
            .AddOkButton()
            .AddButton("openFolderButton", "Open destination _folder", (d, __) =>
            {
                _ = Process.Start("explorer.exe", path);
                d.Close();
            })
            .Show();
        }
        catch (Exception ex)
        {
            TaskDialog.Error(ex.GetBaseException().Message);
        }
    }

    private string SaveGeneratedCodesToDisk()
    {
        var codes = this.GenerateCode();
        return CodeFileService.SaveToDisk(codes.Compact().Select(c => (c, ProjectLayer.ApplicationModel))).ThrowOnFail().GetValue();
    }

    private DtoViewModel MapDtoToViewModel(Dto dto)
    {
        _ = int.TryParse(dto.DbObjectId, out var objectId);
        return new DtoViewModel
        {
            Id = dto.Id,
            Name = dto.Name,
            Comments = dto.Comment,
            NameSpace = dto.Namespace,
            IsList = dto.IsList ?? false,
            IsParams = dto.IsParamsDto,
            IsResult = dto.IsResultDto,
            IsViewModel = dto.IsViewModel,
            ObjectId = objectId,
            Module = this.StaticViewModel?.Modules.FirstOrDefault(m => m.Id == dto.ModuleId),
            Properties = new ObservableCollection<Property>(dto.Properties)
        };
    }
}

public sealed partial class DtoManagementPageStaticViewModel(
    IEnumerable<ModuleViewModel> modules,
    IEnumerable<string> dataTypes,
    IEnumerable<DtoViewModel> dtos)
{
    public ObservableCollection<string> DataTypes { get; } = new(dataTypes);
    public ObservableCollection<ModuleViewModel> Modules { get; } = new(modules);
    public ObservableCollection<DtoViewModel> Dtos { get; } = new(dtos);
}
