using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CodeGenerator.Designer.UI.ViewModels;

public class BasePageViewModel : ViewModelBase
{
    public object? EntityDesignerContent
    {
        get;
        set
        {
            field = value;
            this.OnPropertyChanged();
        }
    }

    public double Progress
    {
        get;
        set
        {
            field = value;
            this.OnPropertyChanged();
        }
    } = 0;

    public object? SidebarContent
    {
        get;
        set
        {
            field = value;
            this.OnPropertyChanged();
        }
    }

    public object? SourceCodeContent
    {
        get;
        set
        {
            field = value;
            this.OnPropertyChanged();
        }
    }

    public string StatusMessage
    {
        get;
        set
        {
            field = value;
            this.OnPropertyChanged();
        }
    } = string.Empty;

    public object? ToolbarContent
    {
        get;
        set
        {
            field = value;
            this.OnPropertyChanged();
        }
    }
}