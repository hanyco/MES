
namespace Library.CodeGenLib.Back;

public interface IStruct : IType
{
    bool IsReadOnly { get; }
}

public sealed class Struct(string name) : TypeBase(name), IStruct
{
    public bool IsReadOnly { get; init; }
}