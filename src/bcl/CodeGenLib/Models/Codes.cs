using System.Collections.ObjectModel;
using System.Numerics;

using Library.Extensions;

namespace Library.CodeGenLib.Models;

public sealed class Codes(params IEnumerable<Code?> items) : ReadOnlyCollection<Code?>([.. items])
    , IEnumerable<Code?>
    , IAdditionOperators<Codes, Codes, Codes>
{
    private static Codes? _empty;

    /// <summary>
    /// Represents an empty instance of Codes class.
    /// </summary>
    public static new Codes Empty { get; } = _empty ??= NewEmpty();

    /// <summary>
    /// Gets the Code item with the specified name.
    /// </summary>
    /// <param name="name">The name of the Code item.</param>
    /// <returns>The Code item with the specified name. If no such item exists, returns null.</returns>
    public Code? this[string name] => this.SingleOrDefault(x => x?.Name == name);

    /// <summary>
    /// Combines two Codes instances into one.
    /// </summary>
    /// <param name="x">The first Codes instance.</param>
    /// <param name="y">The second Codes instance.</param>
    /// <returns>A new Codes instance that combines the Code items from both input instances.</returns>
    public static Codes Combine(Codes x, Codes y) =>
        x + y;

    /// <summary>
    /// Creates a new instance of the Codes class.
    /// </summary>
    /// <returns>A new instance of the Codes class.</returns>
    public static Codes Create(params IEnumerable<Code> arg) =>
        new(arg);

    public static Codes Create(params IEnumerable<Codes> arg) =>
        new(arg.SelectAll());

    /// <summary>
    /// Creates a new instance of the Codes class that is empty.
    /// </summary>
    /// <returns>A new empty instance of the Codes class.</returns>
    public static Codes NewEmpty() =>
        new();

    /// <summary>
    /// Combines two Codes instances into one.
    /// </summary>
    /// <param name="c1">The first Codes instance.</param>
    /// <param name="c2">The second Codes instance.</param>
    /// <returns>A new Codes instance that combines the Code items from both input instances.</returns>
    public static Codes operator +(Codes c1, Codes c2) =>
        new(c1.Concat(c2));

    public override string? ToString() =>
        this.Count == 1 ? this[0]!.ToString() : $"{nameof(Codes)} ({this.Count})";
}