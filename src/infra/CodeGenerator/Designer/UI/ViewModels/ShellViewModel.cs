using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;

namespace CodeGenerator.Designer.UI.ViewModels;

/// <summary>
/// Represents the view model for the shell of the application, managing features and their
/// associated view models.
/// </summary>
/// <remarks>
/// The <see cref="ShellViewModel" /> class is responsible for discovering features within the
/// application assembly, initializing them, and exposing them as a collection. It also tracks the
/// currently selected feature and its associated view model, providing a mechanism for property
/// change notifications.
/// </remarks>
public partial class ShellViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public ShellViewModel()
    {
        var features = new List<FeatureItem>();
        var asm = Assembly.GetExecutingAssembly();
        foreach (var type in asm.GetTypes())
        {
            var attr = type.GetCustomAttribute<FeatureAttribute>();
            if (attr is not null)
            {
                if (Activator.CreateInstance(type) is { } vm)
                {
                    features.Add(new FeatureItem(attr.Title, vm));
                }
            }
        }

        this.Features = new ObservableCollection<FeatureItem>(features);
        this.SelectedFeature = this.Features.FirstOrDefault();
    }

    public ObservableCollection<FeatureItem> Features { get; }

    public FeatureItem? SelectedFeature
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                this.OnPropertyChanged(nameof(this.SelectedFeature));
                this.OnPropertyChanged(nameof(this.SelectedViewModel));
            }
        }
    }

    public object? SelectedViewModel => this.SelectedFeature?.ViewModel;

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
        => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}