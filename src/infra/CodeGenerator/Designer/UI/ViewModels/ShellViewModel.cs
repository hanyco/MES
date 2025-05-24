using System.ComponentModel;
using System.Windows.Input;

using CodeGenerator.Designer.UI.Common;

namespace CodeGenerator.Designer.UI.ViewModels;

/// <summary>
/// Hosts application-level state and navigation.
/// </summary>
public class ShellViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public ShellViewModel(DtosPageViewModel dtosPage)
    {
        // Set initial page
        this.CurrentPageViewModel = dtosPage;

        // Navigation commands
        this.NavigateDtosCommand = new RelayCommand(_ => this.CurrentPageViewModel = dtosPage);
    }

    /// <summary>
    /// The currently displayed page's view model.
    /// </summary>
    public object CurrentPageViewModel
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                this.OnPropertyChanged(nameof(this.CurrentPageViewModel));
            }
        }
    }

    public ICommand NavigateDtosCommand { get; }

    /// <summary>
    /// Status message shown in the StatusBar.
    /// </summary>
    public string StatusMessage
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                this.OnPropertyChanged(nameof(this.StatusMessage));
            }
        }
    } = string.Empty;

    private void OnPropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}