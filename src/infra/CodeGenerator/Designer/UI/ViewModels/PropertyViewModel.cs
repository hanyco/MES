namespace CodeGenerator.Designer.UI.ViewModels;

public partial class PropertyViewModel : ViewModelBase
{
    public string? Comment
    {
        get;
        set
        {
            field = value;
            this.OnPropertyChanged();
        }
    }

    public string? DbObjectId
    {
        get;
        set
        {
            field = value;
            this.OnPropertyChanged();
        }
    }

    public long? DtoId
    {
        get;
        set
        {
            field = value;
            this.OnPropertyChanged();
        }
    }

    public Guid Guid
    {
        get;
        set
        {
            field = value;
            this.OnPropertyChanged();
        }
    }

    public bool? HasGetter
    {
        get;
        set
        {
            field = value;
            this.OnPropertyChanged();
        }
    }

    public bool? HasSetter
    {
        get;
        set
        {
            field = value;
            this.OnPropertyChanged();
        }
    }

    public bool? IsList
    {
        get;
        set
        {
            field = value;
            this.OnPropertyChanged();
        }
    }

    public bool? IsNullable
    {
        get;
        set
        {
            field = value;
            this.OnPropertyChanged();
        }
    }

    public long ParentEntityId
    {
        get;
        set
        {
            field = value;
            this.OnPropertyChanged();
        }
    }

    public int PropertyType
    {
        get;
        set
        {
            field = value;
            this.OnPropertyChanged();
        }
    }

    public string? TypeFullName
    {
        get;
        set
        {
            field = value;
            this.OnPropertyChanged();
        }
    }
}