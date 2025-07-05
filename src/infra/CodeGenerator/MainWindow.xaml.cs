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
    private bool _allowClose;
    public RelayCommand ExitCommand { get; }

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
        ExitCommand = new RelayCommand(_ => ExitApplication());
    }

    partial void OnExitConfirmed();

    private bool ConfirmExit()
    {
        using var dialog = TaskDialogExtension.Create()
            .WithInstructionText("Are you sure you want to exit?")
            .WithCaption("Confirm Exit")
            .WithButtons(TaskDialogStandardButtons.Yes | TaskDialogStandardButtons.No)
            .WithIcon(TaskDialogStandardIcon.Warning);
        if (dialog.Show() is TaskDialogResult.Yes)
        {
            OnExitConfirmed();
            return true;
        }
        return false;
    }

    private void ExitApplication()
    {
        if (ConfirmExit())
        {
            _allowClose = true;
            Application.Current.Shutdown();
        }
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        if (!_allowClose)
        {
            e.Cancel = !ConfirmExit();
            if (!e.Cancel)
                _allowClose = true;
        }
        base.OnClosing(e);
    }
}

