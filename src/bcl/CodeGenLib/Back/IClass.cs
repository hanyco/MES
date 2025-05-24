namespace Library.CodeGenLib.Back;

public interface IClass : IType, IHasGenericTypes
{
    bool IsStatic { get; }

    static IClass New(string name) =>
        new Class(name);
}

public sealed class Class(string name) : TypeBase(name), IClass
{
    public ISet<IGenericType> GenericTypes { get; } = new HashSet<IGenericType>();
    public bool IsStatic { get; init; }
}