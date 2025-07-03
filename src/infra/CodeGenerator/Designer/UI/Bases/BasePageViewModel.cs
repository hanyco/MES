using System.ComponentModel;

namespace CodeGenerator.Designer.UI.Bases;

public class BasePageViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public object? EntityDesignerContent
    {
        get;
        set
        {
            field = value;
            this.OnPropertyChanged(nameof(this.EntityDesignerContent));
        }
    }

    public double Progress
    {
        get;
        set
        {
            field = value;
            this.OnPropertyChanged(nameof(this.Progress));
        }
    } = 0;

    public object? SidebarContent
    {
        get;
        set
        {
            field = value;
            this.OnPropertyChanged(nameof(this.SidebarContent));
        }
    }

    public object? SourceCodeContent
    {
        get;
        set
        {
            field = value;
            this.OnPropertyChanged(nameof(this.SourceCodeContent));
        }
    }

    public string StatusMessage
    {
        get;
        set
        {
            field = value;
            this.OnPropertyChanged(nameof(this.StatusMessage));
        }
    } = string.Empty;

    public object? ToolbarContent
    {
        get;
        set
        {
            field = value;
            this.OnPropertyChanged(nameof(this.ToolbarContent));
        }
    }

    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}