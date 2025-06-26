using System.Collections.ObjectModel;
using System.Numerics;

using Library.CodeGenLib.Extensions;

namespace Library.CodeGenLib.Models;

public sealed class Codes(params IEnumerable<Code?> items) : ReadOnlyCollection<Code?>([.. items])
    , IEnumerable<Code?>
    , IAdditionOperators<Codes, Codes, Codes>
{
    private static Codes? _empty;

    /// <summary>
    /// Represents an empty instance of Codes class.
    /// </summary>
    public static new Codes Empty { get; } = _empty ??= Codes.NewEmpty();

    /// <summary>
    /// Gets the Code item with the specified name.
    /// </summary>
    /// <param name="name"> The name of the Code item. </param>
    /// <returns> The Code item with the specified name. If no such item exists, returns null. </returns>
    public Code? this[string name] => this.SingleOrDefault(x => x?.Name == name);

    /// <summary>
    /// Combines two Codes instances into one.
    /// </summary>
    /// <param name="c1"> The first Codes instance. </param>
    /// <param name="c2"> The second Codes instance. </param>
    /// <returns> A new Codes instance that combines the Code items from both input instances. </returns>
    public static Codes operator +(Codes c1, Codes c2) =>
        new(c1.Concat(c2));

    public override string? ToString() =>
        this.Count == 1 ? this[0]!.ToString() : $"{nameof(Codes)} ({this.Count})";
}