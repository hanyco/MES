using System.ComponentModel;
using System.Windows;

using CodeGenerator.Designer.UI.Common;
using CodeGenerator.Designer.UI.Pages;
using CodeGenerator.Designer.UI.Controls;
using CodeGenerator.UI.Dialogs;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace CodeGenerator;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow(IServiceProvider services)
    {
        this.InitializeComponent();
        this.DataContext = this;

        this.ExitCommand = new RelayCommand(_ => this.ExitApplication(), _ => this._allowClose);
        this._services = services;
    }

    #region Exit Application

    private readonly IServiceProvider _services;
    private bool _allowClose = true;
    public RelayCommand ExitCommand { get; }

    protected override void OnClosing(CancelEventArgs e)
    {
        if (this.ExitCommand.CanExecute(null) && ConfirmExit())
        {
            e.Cancel = false;
        }
        else
        {
            e.Cancel = true;
        }
        base.OnClosing(e);
    }

    private static bool ConfirmExit()
    {
        var response = TaskDialog.AskWithWarning("Are you sure you want to exit?", "Confirm Exit");
        return response is TaskDialogResult.Yes;
    }

    private void ExitApplication()
    {
        if (ConfirmExit())
        {
            this._allowClose = true;
            System.Windows.Application.Current.Shutdown();
        }
    }

    #endregion Exit Application

    private void MenuItem_Click(object sender, RoutedEventArgs e)
        => this.MainContent.Content = this._services.GetService<DtoManagementPage>();

    private void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
        => this.MainContent.Content = this._services.GetService<SettingsControl>();

    private void AboutMenuItem_Click(object sender, RoutedEventArgs e) =>
        TaskDialog.Create()
            .WithCaption("About HanyCo Code Generator")
            .WithInstructionText("HanyCo Infrastructure Code Generator")
            .WithText("""
                HanyCo Code Generator Enterprise 2025 (64-bit) - Preview
                Version 2.20.10 Preview 1.0
                @ 2025 Hamoonazeh Corporation.
                All rights reserved.
                """)
            .WithFooter("Warning: HanyCo Code Generator is protected by copyright law and international treaties. Unauthorized reproduction or distribution of this program, or any portion of it, may result in severe civil and criminal penalties, and will be prosecuted to the maximum extent possible under the law.", TaskDialogStandardIcon.Warning)
            .Show();
}