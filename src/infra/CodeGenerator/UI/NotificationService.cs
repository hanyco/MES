namespace CodeGenerator.UI;

using CommunityToolkit.WinUI.Notifications;
using Windows.UI.Notifications;

public static partial class NotificationService
{
    public static void ShowToast(string title, string message)
    {
        var content = new ToastContentBuilder()
            .AddText(title)
            .AddText(message)
            .GetToastContent();

        var notification = new ToastNotification(content.GetXml());
        ToastNotificationManager.CreateToastNotifier().Show(notification);
    }
}
