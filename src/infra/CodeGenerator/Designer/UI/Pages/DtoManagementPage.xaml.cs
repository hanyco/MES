using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
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

    private async void SaveToDbButton_Click(object sender, RoutedEventArgs e)
    {
        try
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
        catch (Exception ex)
        {
            TaskDialog.Error(ex);
        }
    }

    private void SaveGeneratedCodesToDisk_Click(object sender, RoutedEventArgs e)
        => this.SaveGeneratedCodesToDisk();

    private void SaveGeneratedCodesToDisk()
    {
        try
        {
            var codes = this.GenerateCode();
            var (dialogResult, selectedPath) = FolderBrowserDialog.Show();
            if (dialogResult != DialogResult.OK)
            {
                return;
            }

            foreach (var code in codes.Compact())
            {
                var path = Path.Combine(selectedPath, code.FileName);
                File.WriteAllText(path, code.Statement);
            }
            _ = TaskDialog.Create()
                .WithIcon(TaskDialogStandardIcon.Information)
                .WithInstructionText("Codes are saved.")
                .WithCaption("Save code")
                .AddOkButton()
                .AddButton("openFolderButton", "Open destination _folder", (d, __) =>
                {
                    _ = Process.Start("explorer.exe", selectedPath);
                    d.Close();
                })
                .Show();
        }
        catch (Exception ex)
        {
            TaskDialog.Error(ex.GetBaseException().Message);
        }
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