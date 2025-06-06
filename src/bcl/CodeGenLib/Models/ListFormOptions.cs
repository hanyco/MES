namespace Library.CodeGenLib.Models;

/// <summary>
/// Options for generating list forms.
/// </summary>
public class ListFormOptions
{
    /// <summary>Show button for creating new DTO.</summary>
    public bool IncludeCreateButton { get; set; }

    /// <summary>Show button for deleting selected DTOs.</summary>
    public bool IncludeDeleteButton { get; set; }

    /// <summary>Allow selecting multiple rows.</summary>
    public bool EnableMultiSelect { get; set; }
}
