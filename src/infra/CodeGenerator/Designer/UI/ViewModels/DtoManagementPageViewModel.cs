namespace CodeGenerator.Designer.UI.ViewModels;

using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

using CodeGenerator.Designer.UI.Common;
using CodeGenerator.UI;
using DataLib;

public partial class DtoManagementPageViewModel : ViewModelBase
{
    public ObservableCollection<PropertyViewModel> Properties { get; } = new();

    public IList<PropertyViewModel> SelectedProperties { get; } = new ObservableCollection<PropertyViewModel>();

    public string Name
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

    public string? Comments
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

    public ObservableCollection<Field> Fields { get; set; } = [];

    public bool IsList
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

    public bool IsParams
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

    public bool IsResult
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

    public bool IsViewModel
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

    public ModuleViewModel? Module
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

    public string NameSpace
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

    public int ObjectId
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

    public string? Schema
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

    public RelayCommand DeletePropertiesCommand { get; }

    public DtoManagementPageViewModel()
    {
        this.DeletePropertiesCommand = new RelayCommand(_ => this.DeleteSelected(), _ => this.SelectedProperties.Count > 0);
        ((INotifyCollectionChanged)this.SelectedProperties).CollectionChanged += (_, _) => this.DeletePropertiesCommand.RaiseCanExecuteChanged();
    }

    private void DeleteSelected()
    {
        if (TaskDialogExtension.AskWithWarning("Delete selected properties?", "Confirm Delete") != Microsoft.WindowsAPICodePack.Dialogs.TaskDialogResult.Yes)
        {
            return;
        }

        foreach (var prop in this.SelectedProperties.ToArray())
        {
            this.Properties.Remove(prop);
        }
        this.SelectedProperties.Clear();
    }
}

