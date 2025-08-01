using CodeGenerator.UI.Internals;

using DialogFileSave = System.Windows.Forms.SaveFileDialog;
using DialogFolderBrowser = System.Windows.Forms.FolderBrowserDialog;

namespace CodeGenerator.UI.Dialogs;

public sealed class SaveFileDialog : CommonDialog<DialogFileSave, SaveFileDialog>
{

}

public sealed class FolderBrowserDialog : CommonDialog<DialogFolderBrowser, FolderBrowserDialog>
{
    public static (DialogResult Result, string SelectedPath) Show()
    {
        using var dialog = new FolderBrowserDialog();
        return (dialog.ShowDialog(), dialog.Dialog.SelectedPath);
    }
}

public enum DialogResult
{
    None = 0,
    OK = 1,
    Cancel = 2,
    Abort = 3,
    Retry = 4,
    Ignore = 5,
    Yes = 6,
    No = 7,
    TryAgain = 10,
    Continue = 11
}