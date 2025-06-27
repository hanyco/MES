using Microsoft.WindowsAPICodePack.Dialogs;

namespace CodeGenerator.UI;

public sealed class MsgBox
{
    private MsgBoxData _data;

    private MsgBox()
        => this._data = new();

    public static MsgBox Create()
            => new();

    public static void ShowInfo(string message, string? caption = null)
        => Create()
            .WithMessage(message)
            .WithCaption(caption)
            .WithIcon(TaskDialogStandardIcon.Information)
            .Show();

    public TaskDialogResult Show()
    {
        using var dialog = new TaskDialog
        {
            Caption = this._data.Caption ?? "Information",
            InstructionText = this._data.Message,
            StandardButtons = this._data.Buttons,
            Icon = this._data.Icon
        };
        return dialog.Show();
    }

    public MsgBox WithButtons(TaskDialogStandardButtons buttons)
    {
        this._data.Buttons = buttons;
        return this;
    }

    public MsgBox WithCaption(string? caption)
    {
        this._data.Caption = caption;
        return this;
    }

    public MsgBox WithIcon(TaskDialogStandardIcon icon)
    {
        this._data.Icon = icon;
        return this;
    }

    public MsgBox WithMessage(string? message)
    {
        this._data.Message = message;
        return this;
    }

    private struct MsgBoxData
    {
        public MsgBoxData()
        {
        }

        public TaskDialogStandardButtons Buttons { get; set; } = TaskDialogStandardButtons.Ok;
        public string? Caption { get; set; }
        public TaskDialogStandardIcon Icon { get; set; } = TaskDialogStandardIcon.Information;
        public string? Message { get; set; }
    }
}