using System.ComponentModel;

using Microsoft.WindowsAPICodePack.Dialogs;

namespace CodeGenerator.UI;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class TaskDialogExtension
{
    extension(TaskDialog)
    {
        public static TaskDialog Create()
            => new();

        public static void Info(string instructionText, string? caption = null)
        {
            using var dialog = Create()
                .WithInstructionText(instructionText)
                .WithCaption(caption ?? "Information")
                .WithIcon(TaskDialogStandardIcon.Information);
            _ = dialog.Show();
        }
        public static void Error(string instructionText, string? caption = null)
        {
            using var dialog = Create()
                .WithInstructionText(instructionText)
                .WithCaption(caption ?? "Error")
                .WithIcon(TaskDialogStandardIcon.Error);
            _ = dialog.Show();
        }
        public static void Warning(string instructionText, string? caption = null)
        {
            using var dialog = Create()
                .WithInstructionText(instructionText)
                .WithCaption(caption ?? "Warning")
                .WithIcon(TaskDialogStandardIcon.Warning);
            _ = dialog.Show();
        }
        public static TaskDialogResult AskWithWarning(string instructionText, string? caption = null)
        {
            using var dialog = Create()
                .WithInstructionText(instructionText)
                .WithCaption(caption ?? "Warning")
                .WithIcon(TaskDialogStandardIcon.Warning)
                .WithButtons(TaskDialogStandardButtons.Yes | TaskDialogStandardButtons.No);
            return dialog.Show();
        }
    }

    extension(TaskDialog @this)
    {
        public TaskDialog WithCaption(string? caption)
        {
            @this.Caption = caption ?? "Information";
            return @this;
        }
        public TaskDialog WithInstructionText(string? instructionText)
        {
            @this.InstructionText = instructionText;
            return @this;
        }
        public TaskDialog WithIcon(TaskDialogStandardIcon icon)
        {
            @this.Icon = icon;
            return @this;
        }
        public TaskDialog WithText(string? message)
        {
            @this.Text = message;
            return @this;
        }
        public TaskDialog WithButtons(TaskDialogStandardButtons buttons)
        {
            @this.StandardButtons = buttons;
            return @this;
        }
    }
}