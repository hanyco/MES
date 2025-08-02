using System.Collections;

using Library.Coding;


namespace Library.CodeGenLib.Models;

/// <summary>
/// Represents a collection of programming languages with predefined instances. Supports enumeration and equality
/// checks.
/// </summary>
public readonly struct Languages : IEnumerable<Language>, IEquatable<Languages>
{
    /// <summary>
    /// Represents the Blazor CodeBehind language with a name and file extension. It is defined as a static readonly
    /// member.
    /// </summary>
    public static readonly Language BlazorCodeBehind = new("Blazor Code", "razor.cs");
    public static readonly Language BlazorFront = new("Blazor HTML", "razor");
    public static readonly Language CSharp = new("C#", "cs");
    public static readonly Language Html = new("HTML", "htm");
    public static readonly Language None = new("(Unknown)", "");
    public static readonly Language Sql = new("SQL", "sql");
    public static readonly Language Xaml = new("XAML", "xaml.cs");

    /// <summary>
    /// Compares two instances of the Languages type for inequality.
    /// </summary>
    /// <param name="left">Represents the first instance to compare.</param>
    /// <param name="right">Represents the second instance to compare.</param>
    /// <returns>Returns true if the instances are not equal; otherwise, false.</returns>
    public static bool operator !=(Languages left, Languages right)
        => !(left == right);

    /// <summary>
    /// Compares two instances of the Languages type for equality.
    /// </summary>
    /// <param name="left">The first instance of the Languages type to compare.</param>
    /// <param name="right">The second instance of the Languages type to compare.</param>
    /// <returns>True if both instances are equal; otherwise, false.</returns>
    public static bool operator ==(Languages left, Languages right)
        => left.Equals(right);

    public override bool Equals(object? obj)
        => base.Equals(obj);

    public bool Equals(Languages other)
        => this == other;

    public IEnumerator<Language> GetEnumerator()
    {
        var me = this;
        return this.GetType()
            .GetFields()
            .Where(x => x.FieldType == typeof(Language))
            .Select(x => x.Cast().To<System.Reflection.FieldInfo>().GetValue(me).Cast().To<Language>())
            .GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
        => this.GetEnumerator();

    public override int GetHashCode()
        => base.GetHashCode();

    public override string? ToString()
        => base.ToString();
}