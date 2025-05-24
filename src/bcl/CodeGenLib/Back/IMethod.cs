using Library.CodeGeneration.Models;
using Library.CodeGenLib.Back;
using Library.Results;
using Library.Validations;

namespace Library.CodeGenLib.Back;

public interface IMethod : IMember, IHasGenericTypes
{
    ISet<MethodArgument> Arguments { get; }
    string? Body { get; }
    bool IsConstructor { get; }
    bool IsExtension { get; }
    TypePath? ReturnType { get; }
    bool IsAsync { get; }

    static IMethod New(string name, string? body = null, IEnumerable<MethodArgument>? arguments = null, TypePath? returnType = null)
    {
        var result = new Method(name)
        {
            Body = body,
            ReturnType = returnType
        };
        if (arguments?.Any() == true)
        {
            _ = result.Arguments.AddRange(arguments);
        }
        return result;
    }
}

public sealed class Method(string name) : Member(name), IMethod
{
    public ISet<MethodArgument> Arguments { get; } = new HashSet<MethodArgument>();
    public string? Body { get; set; }
    public ISet<IGenericType> GenericTypes { get; } = new HashSet<IGenericType>();
    public bool IsConstructor { get; init; }
    public bool IsExtension { get; init; }
    public TypePath? ReturnType { get; init; }
    public bool IsAsync { get; set; }

    protected override Result OnValidate() =>
        this.Check()
            .RuleFor(x => !(x.IsExtension && !x.Arguments.Any()), () => "Extension method cannot be parameterless.")
            .RuleFor(x => !(x.IsConstructor && x.IsExtension), () => "Constructor cannot be extension method.")
            .Build();
}

public static class MethodExtensions
{
    public static TMethod AddArgument<TMethod>(this TMethod method, string Type, string Name) where TMethod : IMethod
        => method.AddArgument(new MethodArgument(Type, Name));

    public static TMethod AddArgument<TMethod>(this TMethod method, params IEnumerable<(TypePath Type, string Name)> arguments) where TMethod : IMethod
    {
        foreach (var argument in arguments)
        {
            _ = method.AddArgument(new MethodArgument(argument.Type, argument.Name));
        }
        return method;
    }

    public static TMethod AddArgument<TMethod>(this TMethod method, MethodArgument argument) where TMethod : IMethod
        => method.Fluent(method.Arguments.Add(argument));

    public static IEnumerable<string> GetNameSpaces(this IMethod method)
    {
        Check.MustBeArgumentNotNull(method);
        return new HashSet<string>(gather(method)).AsEnumerable();

        static IEnumerable<string> gather(IMethod method)
        {
            if (method.ReturnType != null)
            {
                foreach (var item in method.ReturnType.GetNameSpaces())
                {
                    yield return item;
                }
            }

            foreach (var item in method.Arguments.Select(x => x.Type))
            {
                foreach (var ns in item.GetNameSpaces())
                {
                    yield return ns;
                }
            }
        }
    }
}