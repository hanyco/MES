namespace Library.CodeGenLib.Models;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public readonly record struct Language([DisallowNull] in string Name, in string? FileExtension = null) : IEquatable<Language>
{
    public override string ToString() =>
        this.Name;

    private string GetDebuggerDisplay() =>
        this.ToString();
}