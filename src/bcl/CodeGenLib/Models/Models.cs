using System.CodeDom;
using System.Collections.ObjectModel;

using Library.Coding;

namespace Library.CodeGenLib.Models;

public interface IMemberInfo
{
    string Name { get; }
}

public readonly struct FieldInfo(
    in TypePath type,
    in string name,
    in string? comment = null,
    in MemberAttributes? accessModifier = null,
    in bool isReadOnly = false,
    in bool isPartial = false) : IMemberInfo
{
    public MemberAttributes? AccessModifier { get; } = accessModifier;
    public string? Comment { get; } = comment;
    public bool IsPartial { get; } = isPartial;
    public bool IsReadOnly { get; } = isReadOnly;
    public string Name { get; } = name;
    public TypePath Type { get; } = type;

    public static bool operator !=(FieldInfo left, FieldInfo right) => !(left == right);

    public static bool operator ==(FieldInfo left, FieldInfo right) => left.Equals(right);

    public override bool Equals(object obj) => throw new NotImplementedException();

    public override int GetHashCode() => throw new NotImplementedException();
}

public readonly struct MethodArgument(in TypePath type, in string? name) : IEquatable<MethodArgument>
{
    public string Name { get; } = name ?? TypeMemberNameHelper.ToArgName(type.ArgumentNotNull().Name);
    public TypePath Type { get; } = type;

    public static MethodArgument Create(TypePath arg) =>
        new(arg.ArgumentNotNull(), TypeMemberNameHelper.ToArgName(arg.Name));

    public static bool operator !=(MethodArgument left, MethodArgument right) =>
        !(left == right);

    public static bool operator ==(MethodArgument left, MethodArgument right) =>
        left.Equals(right);

    public void Deconstruct(out TypePath type, out string name) =>
        (type, name) = (this.Type, this.Name);

    public override bool Equals(object? obj) =>
        obj is MethodArgument ma && this.Equals(ma);

    public bool Equals(MethodArgument other) =>
        this.GetHashCode() == other.GetHashCode();

    public override int GetHashCode() =>
        HashCode.Combine(this.Type, this.Name);
}

public sealed class PropertyInfo(
    in string type,
    in string name,
    in MemberAttributes? accessModifier = null,
    in PropertyInfoAccessor? getter = null,
    in PropertyInfoAccessor? setter = null) : IMemberInfo
{
    public PropertyInfo()
        : this(string.Empty, string.Empty)
    {
    }

    public MemberAttributes? AccessModifier { get; init; } = accessModifier;
    public Collection<string> Attributes { get; } = [];
    public string? BackingFieldName { get; init; }
    public string? Comment { get; init; }
    public PropertyInfoAccessor? Getter { get; init; } = getter;
    public bool HasBackingField { get; init; }
    public string? InitCode { get; init; }
    public bool IsNullable { get; init; }
    public string Name { get; init; } = name;
    public PropertyInfoAccessor? Setter { get; init; } = setter;
    public TypePath Type { get; init; } = type;
}

public readonly struct PropertyInfoAccessor(in bool has = true, in bool? isPrivate = null, in string? code = null)
{
    public string? Code { get; } = code;
    public bool Has { get; } = has;
    public bool? IsPrivate { get; } = isPrivate;

    public static bool operator !=(PropertyInfoAccessor left, PropertyInfoAccessor right) => !(left == right);

    public static bool operator ==(PropertyInfoAccessor left, PropertyInfoAccessor right) => left.Equals(right);

    public void Destruct(out bool has, out bool? isPrivate, out string? code) =>
        (has, isPrivate, code) = (this.Has, this.IsPrivate, this.Code);

    public override bool Equals(object obj) => throw new NotImplementedException();

    public override int GetHashCode() => throw new NotImplementedException();
}