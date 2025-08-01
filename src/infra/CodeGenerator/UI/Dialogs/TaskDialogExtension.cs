using System.ComponentModel;
using System.Windows.Interop;

using Microsoft.WindowsAPICodePack.Dialogs;

namespace CodeGenerator.UI.Dialogs;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class TaskDialogExtension
{
    extension(TaskDialog)
    {
        public static TaskDialog Create(bool setOwner = true)
        {
            var result = new TaskDialog();
            if (setOwner)
            {
                result.OwnerWindowHandle = new WindowInteropHelper(System.Windows.Application.Current.MainWindow).Handle;
            }
            return result;
        }

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
        public static void Error(Exception exception, string? caption = null)
        {
            using var dialog = Create()
                .WithInstructionText(exception.GetBaseException().Message)
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

        public TaskDialog WithDetailsExpanded(
            string text,
            string? label = null,
            string? collapsedLabel = null,
            bool? detailsExpanded = null,
            TaskDialogExpandedDetailsLocation? mode = null)
        {
            @this.DetailsExpandedText = text;
            if (!label.IsNullOrEmpty())
            {
                @this.DetailsExpandedLabel = label;
            }
            if (!collapsedLabel.IsNullOrEmpty())
            {
                @this.DetailsCollapsedLabel = collapsedLabel;
            }
            if (detailsExpanded is { } expanded)
            {
                @this.DetailsExpanded = expanded;
            }

            if (mode is { } exMode)
            {
                @this.ExpansionMode = exMode;
            }
            return @this;
        }
        public TaskDialog WithFooter(
            string footerText,
            TaskDialogStandardIcon? icon = null,
            bool? checkBoxChecked = null,
            string? checkBoxText = null)
        {
            @this.FooterText = footerText;
            if (icon is { } ic)
            {
                @this.FooterIcon = ic;
            }

            if (checkBoxChecked is { } cbChecked)
            {
                @this.FooterCheckBoxChecked = cbChecked;
            }

            if (checkBoxText is { } cbText)
            {
                @this.FooterCheckBoxText = cbText;
            }

            return @this;
        }

        public TaskDialog WithButtons(TaskDialogStandardButtons buttons)
        {
            @this.StandardButtons = buttons;
            return @this;
        }
        public TaskDialog AddButtons(params IEnumerable<TaskDialogButton> buttons)
        {
            foreach (var button in buttons)
            {
                @this.Controls.Add(button);
            }
            return @this;
        }
        public TaskDialog AddButton(string name, string text, Action<TaskDialog, TaskDialogButton> click, bool isDefault = false) =>
            @this.AddButtons(TaskDialogButton.Create(name, text, click, isDefault));
        public TaskDialog AddButton(string name, string text, Action<TaskDialog> click, bool isDefault = false) =>
            @this.AddButtons(TaskDialogButton.Create(name, text, click, isDefault));
        public TaskDialog AddOkButton(string? name = null, bool isDefault = true) =>
            @this.AddButtons(TaskDialogButton.Create(name ?? "okButton", "Ok", d => d.Close(), isDefault));
    }

    extension(TaskDialogButton)
    {
        public static TaskDialogButton Create(string name, string text, Action<TaskDialog> click, bool isDefault = false) =>
            Create(name, text, (d, _) => click(d), isDefault);
        public static TaskDialogButton Create(string name, string text, Action<TaskDialog, TaskDialogButton> click, bool isDefault = false)
        {
            var result = new TaskDialogButton { Text = text, Default = isDefault };

            if (click != null)
            {
                result.Click += (s, _) =>
                {
                    var btn = s as TaskDialogButton;
                    var dlg = btn?.HostingDialog as TaskDialog;
                    click(dlg!, btn!);
                };
            }
            return result;
        }
    }


}