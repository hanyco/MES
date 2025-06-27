using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace CodeGenerator.Designer.UI.ViewModels;

/// <summary>
/// میزبان وضعیت کلی برنامه و مدیریت ناوبری است.
/// </summary>
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

    private FeatureItem? _selectedFeature;
    public FeatureItem? SelectedFeature
    {
        get => this._selectedFeature;
        set
        {
            if (this._selectedFeature != value)
            {
                this._selectedFeature = value;
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
