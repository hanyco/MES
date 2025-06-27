namespace CodeGenerator.UI;

using Microsoft.WindowsAPICodePack.Dialogs;

public static partial class DialogService
{
    public static void ShowInfo(string message, string? caption = null)
    {
        var dialog = new TaskDialog
        {
            Caption = caption ?? "Info",
            InstructionText = message,
            StandardButtons = TaskDialogStandardButtons.Ok
        };
        dialog.Show();
    }
}
