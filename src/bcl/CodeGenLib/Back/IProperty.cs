using Library.CodeGenLib.Helpers;
using Library.Coding;

namespace Library.CodeGenLib.Back;

public interface IProperty : IMember
{
    string? BackingFieldName { get; }
    PropertyAccessor? Getter { get; }
    PropertyAccessor? Setter { get; }
    TypePath Type { get; }

    static IProperty New(string name, TypePath type)
        => new CodeGenProperty(name, type);

    static IProperty New(string name, TypePath type, string? backingFieldName)
        => new CodeGenProperty(name, type, backingField: backingFieldName ?? TypeMemberNameHelper.ToFieldName(name));
}

public class CodeGenProperty : Member, IProperty
{
    public CodeGenProperty(
    string name,
    TypePath type,
    AccessModifier modifier = AccessModifier.Public,
    InheritanceModifier inheritanceModifier = InheritanceModifier.None,
    PropertyAccessor? setter = null,
    PropertyAccessor? getter = null,
    string? backingField = null) : base(name)
    {
        this.Type = type.ArgumentNotNull();
        this.AccessModifier = modifier;
        this.InheritanceModifier = inheritanceModifier;
        this.BackingFieldName = backingField;
        if (!backingField.IsNullOrEmpty())
        {
            setter ??= new();
            getter ??= new();
        }
        this.Setter = setter;
        this.Getter = getter;
    }

    public string? BackingFieldName { get; }
    public PropertyAccessor? Getter { get; }
    public PropertyAccessor? Setter { get; }
    public TypePath Type { get; }
}

public readonly record struct PropertyAccessor(AccessModifier AccessModifier = AccessModifier.None, string? Body = null);