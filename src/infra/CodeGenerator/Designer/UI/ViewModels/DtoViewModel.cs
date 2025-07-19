using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;

using CodeGenerator.Application.Domain;
using CodeGenerator.Designer.UI.Common;

using DataLib;

using Library.Validations;

using Microsoft.WindowsAPICodePack.Dialogs;

namespace CodeGenerator.Designer.UI.ViewModels;

public sealed partial class DtoViewModel : ViewModelBase
{
    public DtoViewModel()
    {
        this.DeletePropertiesCommand = new(_ => this.DeleteSelectedProperties(), _ => this.SelectedProperties.Count > 0);
        if (this.SelectedProperties is INotifyCollectionChanged collection)
        {
            collection.CollectionChanged += (_, _) => this.DeletePropertiesCommand.RaiseCanExecuteChanged();
        }
    }

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

    public RelayCommand DeletePropertiesCommand { get; }

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

    public ObservableCollection<Property> Properties { get; set; } = [];

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

    public IList<Property> SelectedProperties { get; } = new ObservableCollection<Property>();

    public static DtoViewModel CrateByTable(Table table) => new()
    {
        Properties = new(table.Fields.Select(Property.GetByTableField)),
        Name = table.Name,
        ObjectId = table.ObjectId,
        Schema = table.Schema,
    };

    private void DeleteSelectedProperties()
    {
        if (this.SelectedProperties.Count == 0)
        {
            return;
        }

        var message = $"Delete {this.SelectedProperties.Count} selected properties?";
        var resp = TaskDialog.AskWithWarning(message, "Delete Confirmation");
        if (resp != TaskDialogResult.Yes)
        {
            return;
        }

        foreach (var field in this.SelectedProperties.Cast<Property>().ToList())
        {
            _ = this.Properties.Remove(field);
        }
        this.SelectedProperties.Clear();
    }
}