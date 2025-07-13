using System.Collections;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Input;

using DataLib;
using CodeGenerator.Designer.UI.Common;

namespace CodeGenerator.Designer.UI.ViewModels;

public sealed partial class DtoManagementPageViewModel : ViewModelBase
{
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

    public IList<Field> SelectedProperties { get; } = new ObservableCollection<Field>();

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
        this.DeletePropertiesCommand = new(_ => this.DeleteSelectedProperties(), _ => this.SelectedProperties.Count > 0);
        if (this.SelectedProperties is INotifyCollectionChanged collection)
        {
            collection.CollectionChanged += (_, _) => this.DeletePropertiesCommand.RaiseCanExecuteChanged();
        }
    }

    private void DeleteSelectedProperties()
    {
        if (this.SelectedProperties.Count == 0)
        {
            return;
        }

        var message = $"Delete {this.SelectedProperties.Count} selected properties?";
        if (MessageBox.Show(message, "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
        {
            return;
        }

        foreach (var field in this.SelectedProperties.Cast<Field>().ToList())
        {
            _ = this.Fields.Remove(field);
        }
        this.SelectedProperties.Clear();
    }

    public static DtoManagementPageViewModel CrateByTable(Table table) => new()
    {
        Fields = new(table.Fields),
        Name = table.Name,
        ObjectId = table.ObjectId,
        Schema = table.Schema,
    };

    public Table BuildTable() => new()
    {
        Fields = ImmutableArray.Create(this.Fields.ToArray()),
        Name = this.Name,
        Schema = this.Schema,
        ObjectId = this.ObjectId,
    };
}