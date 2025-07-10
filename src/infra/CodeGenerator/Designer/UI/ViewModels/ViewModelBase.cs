using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CodeGenerator.Designer.UI.ViewModels;

public abstract class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public long? Id
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                this.OnPropertyChanged();
            }
        }
    }

    public string? Name
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                this.OnPropertyChanged();
            }
        }
    } = string.Empty;

    public override string? ToString() => this switch
    {
        { Name: not null } => this.Name,
        { Id: not null } => this.Id.ToString(),
        _ => base.ToString()
    };

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}