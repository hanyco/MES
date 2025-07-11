using System.ComponentModel;
using System.Windows;

using CodeGenerator.Designer.UI.Common;
using CodeGenerator.Designer.UI.Pages;
using CodeGenerator.UI;

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
}