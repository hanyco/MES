using System.ComponentModel;
using System.Windows.Input;

using CodeGenerator.Designer.UI.Common;

namespace CodeGenerator.Designer.UI.ViewModels;

/// <summary>
/// Hosts application-level state and navigation.
/// </summary>
public class ShellViewModel : INotifyPropertyChanged
{
    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShellViewModel" /> class.
    /// </summary>
    /// <param name="dtosPage"> The view model for the DTOs page. </param>
    public ShellViewModel(DtosPageViewModel dtosPage)
    {
        // Set initial page
        this.CurrentPageViewModel = dtosPage;

        // Navigation commands
        this.NavigateDtosCommand = new RelayCommand(_ => this.CurrentPageViewModel = dtosPage);
    }

    /// <summary>
    /// Gets or sets the currently displayed page's view model.
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

    /// <summary>
    /// Gets the command to navigate to the DTOs page.
    /// </summary>
    public ICommand NavigateDtosCommand { get; }

    /// <summary>
    /// Gets or sets the status message shown in the StatusBar.
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

    /// <summary>
    /// Raises the <see cref="PropertyChanged" /> event for the specified property.
    /// </summary>
    /// <param name="propertyName"> The name of the property that changed. </param>
    private void OnPropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}