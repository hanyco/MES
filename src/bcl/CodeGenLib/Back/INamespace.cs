using Library.CodeGeneration.Models;
using Library.CodeGenLib;
using Library.DesignPatterns.Markers;
using Library.Results;
using Library.Validations;

namespace Library.CodeGenLib.Back;

public interface INamespace : IValidatable
{
    string Name { get; }
    ISet<IType> Types { get; }
    ISet<string> UsingNamespaces { get; }

    static INamespace New(string name) =>
        new Namespace(name);
}

[Immutable]
public sealed class Namespace(string name) : INamespace
{
    public string Name { get; } = name.ArgumentNotNull();
    public ISet<IType> Types { get; } = new HashSet<IType>();
    public ISet<string> UsingNamespaces { get; } = new HashSet<string>();

    public Result Validate()
    {
        if (this.UsingNamespaces.Any(x => x.IsNullOrEmpty()))
        {
            return Result.Fail("Using namespace cannot be empty.");
        }
        if (this.Types.Any(x => x == null))
        {
            return Result.Fail("Type cannot be empty.");
        }
        foreach (var type in this.Types)
        {
            if (!type.Validate().TryParse(out var vr))
            {
                return vr;
            }
        }
        return Result.Succeed;
    }
}

public static class NamSpaceExtensions
{
    public static TNameSpace AddType<TNameSpace>(this TNameSpace nameSpace, params IType[] types) where TNameSpace : INamespace
    {
        foreach (var type in types.Compact())
        {
            _ = nameSpace.Types.Add(type);
        }
        return nameSpace;
    }

    public static TNameSpace AddUsingNameSpace<TNameSpace>(this TNameSpace ns, params IEnumerable<string> nameSpaces) where TNameSpace : INamespace
    {
        nameSpaces.Distinct().Compact().ForEach(x => ns.UsingNamespaces.Add(x));
        return ns;
    }

    public static Result<string> GenerateCode<TCodeGeneratorEngine>(this INamespace ns, TCodeGeneratorEngine engine)
        where TCodeGeneratorEngine : ICodeGeneratorEngine => engine.Generate(ns);

    public static Result<string> GenerateCode<TCodeGeneratorEngine>(this INamespace ns)
        where TCodeGeneratorEngine : ICodeGeneratorEngine, new()
        => ns.GenerateCode(new TCodeGeneratorEngine());
}