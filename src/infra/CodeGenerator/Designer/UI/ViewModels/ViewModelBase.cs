using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CodeGenerator.Designer.UI.ViewModels;

public partial class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
