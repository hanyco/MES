using Library.Validations;

namespace Library.Globalization.Attributes;

/// <summary>
///     Used to localize the members of a class
/// </summary>
/// <seealso cref="System.Attribute" />
[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
internal sealed class LocalizedDescriptionAttribute : Attribute
{
    public LocalizedDescriptionAttribute(string cultureName)
        : this(cultureName, string.Empty)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="LocalizedDescriptionAttribute" /> class.
    /// </summary>
    /// <param name="cultureName">Name of the culture.</param>
    /// <param name="description">The description.</param>
    /// <exception cref="ArgumentNullException">cultureName</exception>
    public LocalizedDescriptionAttribute(string cultureName, string? description)
    {
        this.CultureName = cultureName.NotNull();
        this.Description = description;
    }

    /// <summary>
    ///     Gets or sets the name of the culture.
    /// </summary>
    /// <value>
    ///     The name of the culture.
    /// </value>
    public string CultureName { get; set; }

    /// <summary>
    ///     Gets or sets the description.
    /// </summary>
    /// <value>
    ///     The description.
    /// </value>
    public string? Description { get; set; }
}
