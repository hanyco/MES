namespace Library.CodeGenLib.Back;

public interface IGenericType
{
    string Constraints { get; }
    string Name { get; }
}

public interface IHasGenericTypes
{
    ISet<IGenericType> GenericTypes { get; }
}

public interface IHasAttributes
{
    ISet<ICodeGenAttribute> Attributes { get; }
}