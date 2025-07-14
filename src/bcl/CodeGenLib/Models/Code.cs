using Library.Dynamics;

namespace Library.CodeGenLib.Models;

[DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
public class Code([DisallowNull] in string name, [DisallowNull] in Language language, [DisallowNull] in string statement, in bool isPartial = false, in string? fileName = null)
    : IEquatable<Code>
    , IComparable<Code>
{
    private static Code? _empty;

    /// <summary>
    /// Constructor for the <see cref="Code" /> with parameters.
    /// </summary>
    /// <param name="name">      Name of the code. </param>
    /// <param name="language">  Language of the code. </param>
    /// <param name="statement"> Statement of the code. </param>
    /// <param name="isPartial"> Whether the code is partial or not. </param>
    /// <param name="fileName">  File name of the code. </param>
    /// <returns> An instance of the <see cref="Code" />. </returns>
    public Code(in (string Name, Language Language, string Statement, bool IsPartial, string? FileName) data)
        : this(data.Name, data.Language, data.Statement, data.IsPartial, data.FileName) { }

    /// <summary>
    /// Constructor for the <see cref="Code" /> with a copy of the original Code instance.
    /// </summary>
    /// <param name="original"> Original Code instance to copy. </param>
    public Code(Code original)
        : this(original.Name, original.Language, original.Statement, original.IsPartial, original.FileName)
    {
    }

    /// <summary>
    /// Represents an empty instance of <see cref="Code" /> class.
    /// </summary>
    public static Code Empty { get; } = _empty ??= new(string.Empty, Languages.None, string.Empty);

    /// <summary>
    /// Gets a collection of additional properties that can be dynamically added or accessed at runtime.
    /// </summary>
    /// <remarks>
    /// This property is useful for scenarios where flexible, runtime-defined data needs to be
    /// associated with an object. Properties can be added or accessed using dynamic syntax. Ensure
    /// proper validation when using dynamic properties to avoid runtime errors.
    /// </remarks>
    public dynamic ExtraProperties { get; } = new Expando();

    /// <summary>
    /// Gets the file name of the code. If the file name is null or empty, generate a new file name
    /// based on the code's name, language, and partial status.
    /// </summary>
    public string FileName { get => field.IfNullOrEmpty(GenerateFileName(this.Name, this.Language, this.IsPartial)); } = fileName;

    // Backup: public string FileName { get => field.IfNullOrEmpty(GenerateFileName(this.Name, this.Language, this.IsPartial)); } = fileName;

    /// <summary>
    /// Gets whether the code is partial or not.
    /// </summary>
    public bool IsPartial { get; } = isPartial;

    /// <summary>
    /// Gets the language of the code.
    /// </summary>
    public Language Language { get; } = language;

    /// <summary>
    /// Gets the name of the code.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// Gets or sets the statement of the code.
    /// </summary>
    public string Statement { get; init; } = statement;

    public static implicit operator Code(in (string Name, Language language, string Statement, bool IsPartial) codeData) =>
        new(codeData);

    public static implicit operator Code(in (string Name, Language language, string Statement, bool IsPartial, string FileName) codeData) =>
        new(codeData.Name, codeData.language, codeData.Statement, codeData.IsPartial, codeData.FileName);

    public static implicit operator string?(in Code? code) =>
        code?.Statement;

    public static bool operator !=(Code? left, Code? right) =>
        !(left == right);

    public static bool operator <(Code left, Code right) => left is null ? right is not null : left.CompareTo(right) < 0;

    public static bool operator <=(Code left, Code right) => left is null || left.CompareTo(right) <= 0;

    public static bool operator ==(Code? left, Code? right) =>
                left?.Equals(right) ?? right is null;

    public static bool operator >(Code left, Code right) => left is not null && left.CompareTo(right) > 0;

    public static bool operator >=(Code left, Code right) => left is null ? right is null : left.CompareTo(right) >= 0;

    public int CompareTo(Code? other) =>
        other is null ? 1 : other.Name.CompareTo(this.Name);

    public virtual bool Equals(Code? other) =>
        this.GetHashCode() == other?.GetHashCode();

    public override bool Equals(object? obj) =>
        this.Equals(obj as Code);

    public override int GetHashCode() =>
        HashCode.Combine(this.Name, this.Language);

    public override string ToString() =>
        this.Name;

    private static string GenerateFileName(string name, Language language, bool isPartial) =>
        Path.ChangeExtension(isPartial ? $"{name}.partial.tmp" : name, language.FileExtension);

    private string GetDebuggerDisplay() =>
        this.Name;
}