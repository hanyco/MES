using System.Collections.Immutable;
using System.Collections.ObjectModel;

using DataLib;

using Library.Validations;

namespace CodeGenerator.Designer.UI.ViewModels;

public sealed class DtoManagementPageViewModel : ViewModelBase
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
        Name = this.Name.NotNull(),
        Schema = this.Schema,
        ObjectId = this.ObjectId,
    };
}