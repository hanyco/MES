using Library.Coding;

namespace Library.CodeGenLib.Back;

public interface IType : IValidatable, IHasAttributes
{
    AccessModifier AccessModifier { get; }
    ISet<TypePath> BaseTypes { get; }
    InheritanceModifier InheritanceModifier { get; }
    ISet<IMember> Members { get; }
    string Name { get; }
    ISet<string> UsingNamesSpaces { get; }
}

public abstract class TypeBase(in string name) : IType
{
    public virtual AccessModifier AccessModifier { get; init; } = AccessModifier.Public;
    public ISet<ICodeGenAttribute> Attributes { get; } = new HashSet<ICodeGenAttribute>();
    public virtual ISet<TypePath> BaseTypes { get; } = new HashSet<TypePath>();
    public virtual InheritanceModifier InheritanceModifier { get; init; } = InheritanceModifier.Sealed;
    public virtual ISet<IMember> Members { get; } = new HashSet<IMember>();
    public virtual string Name { get; } = name;
    public virtual ISet<string> UsingNamesSpaces { get; } = new HashSet<string>([typeof(string).Namespace, typeof(Enumerable).Namespace, typeof(Task).Namespace,]);

    public virtual Result Validate() =>
        Result.Success();
}

public static class TypeExtensions
{
    public static TType AddBaseType<TType>(this TType type, TypePath baseType) where TType : IType
    {
        _ = (type?.BaseTypes.Add(baseType));
        return type;
    }

    public static IClass AddBaseType<TBaseType>(this IClass type)
        => type.AddBaseType(typeof(TBaseType));

    public static TType AddField<TType>(this TType type, string name, TypePath typePath, AccessModifier? accessModifier = IField.DefaultAccessModifier) where TType : IType =>
        type.AddMember(IField.New(name, typePath, AccessModifier.Private));

    //public static TType AddMember<TType>(this TType type, params IMember[] members)
    //        where TType : IType
    //{
    //    if (members?.Any() == true)
    //    {
    //        foreach (var member in members)
    //        {
    //            _ = type.Members.Add(member);
    //        }
    //    }

    //    return type;
    //}

    public static TType AddMember<TType>(this TType type, params IEnumerable<IMember> members) where TType : IType
    {
        if (members?.Any() == true)
        {
            foreach (var member in members)
            {
                _ = type.Members.Add(member);
            }
        }

        return type;
    }

    public static TType AddMember<TType>(this TType type, IMember member) where TType : IType
    {
        _ = type.Members.Add(member);
        return type;
    }

    public static TType AddMethod<TType>(this TType type, Method method) where TType : IType =>
        type.AddMember(method);

    public static TType AddMethod<TType>(this TType type, string name, string? body = null, IEnumerable<(TypePath Type, string Name)>? parameters = null, TypePath? returnType = null) where TType : IType =>
        type.AddMember(IMethod.New(name, body, parameters?.Select(x => new Models.MethodArgument(x.Type, x.Name)), returnType));

    public static TType AddProperty<TType>(this TType type, string name, TypePath typePath) where TType : IType =>
        type.AddMember(IProperty.New(name, typePath));

    public static TType AddProperty<TType>(this TType type, IProperty property) where TType : IType =>
        type.AddMember(property);

    public static TType AddProperty<TType>(this TType type, string name, TypePath typePath, string backingFieldName) where TType : IType =>
        type.AddMember(IProperty.New(name, typePath, backingFieldName));
}