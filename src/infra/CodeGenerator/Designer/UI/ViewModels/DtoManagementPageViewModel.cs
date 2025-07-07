using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using DataLib;

namespace CodeGenerator.Designer.UI.ViewModels;

public sealed class DtoManagementPageViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

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

    public ObservableCollection<Field> Fields
    {
        get;
        set
        {
            if (!field.Equals(value))
            {
                field = value;
                this.OnPropertyChanged();
            }
        }
    } = [];

    public long Id
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

    public IEnumerable<ModuleViewModel> Modules
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

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}