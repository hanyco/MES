namespace CodeGenerator.UI;

using Microsoft.WindowsAPICodePack.Taskbar;

public static partial class TaskbarService
{
    public static TaskbarManager Manager => TaskbarManager.Instance;
}
