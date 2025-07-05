using System.ComponentModel;
using System.Windows;

using CodeGenerator.Designer.UI.Common;
using CodeGenerator.UI;

using Microsoft.WindowsAPICodePack.Dialogs;

namespace CodeGenerator;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private bool _allowClose = true;

    public MainWindow()
    {
        this.InitializeComponent();
        this.DataContext = this;

        this.ExitCommand = new RelayCommand(_ => this.ExitApplication(), _ => this._allowClose);
    }

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
}