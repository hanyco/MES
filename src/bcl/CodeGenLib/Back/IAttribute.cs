namespace Library.CodeGenLib.Back;

public interface ICodeGenAttribute
{
    TypePath Name { get; }
    ISet<(string? Name, string Value)> Properties { get; }

    static ICodeGenAttribute New(TypePath name) =>
        new CodeGenAttribute(name);

    static ICodeGenAttribute New(TypePath name, params IEnumerable<(string? Name, string Value)> properties) =>
        new CodeGenAttribute(name, properties);
}

public class CodeGenAttribute(TypePath name) : ICodeGenAttribute
{
    public CodeGenAttribute(TypePath name, params IEnumerable<(string? Name, string Value)> properties) : this(name) =>
        this.Properties.AddRange(properties);

    public TypePath Name { get; } = name;
    public ISet<(string? Name, string Value)> Properties { get; } = new HashSet<(string? Name, string Value)>();
}