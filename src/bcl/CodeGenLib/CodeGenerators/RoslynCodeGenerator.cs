using Library.Coding;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using static Library.Extensions.RoslynHelper;

namespace Library.CodeGenLib.CodeGenerators;

public sealed class RoslynCodeGenerator : ICodeGeneratorEngine<INamespace>, ICodeGeneratorEngine<IProperty>
{
    public IResult<string> Generate(IProperty property)
    {
        Check.MustBeArgumentNotNull(property);

        var prop = CreateRosProperty(CreateRoot(), property);
        var statement = prop.Root.AddMembers(prop.Member).GenerateCode();
        var result = Result.Success(ReformatCode(statement));
        return result;
    }

    public IResult<string> Generate(INamespace nameSpace)
    {
        // Validation checks
        Check.MustBeArgumentNotNull(nameSpace);
        if (!nameSpace.Validate().TryParse(out var vr))
        {
            return vr.WithValue(string.Empty);
        }

        // Create compilation unit
        var root = CreateRoot();

        // Create namespace
        var rosNameSpace = CreateNamespace(nameSpace.Name);

        // Add types
        foreach (var type in nameSpace.Types)
        {
            var modifiers = GeneratorHelper.ToModifiers(type.AccessModifier, type.InheritanceModifier);
            // Create type
            var rosType = CreateType(TypePath.New(type.Name), modifiers);

            // Add base class or interfaces to type
            foreach (var baseType in type.BaseTypes)
            {
                rosType = rosType.AddBase(baseType.FullName);
                root = baseType.GetNameSpaces().SelectImmutable((ns, r) => r.AddUsingNameSpace(ns), root);
            }

            // Add attributes
            foreach (var attribute in type.Attributes.Compact())
            {
                var attributeType = TypePath.New(attribute.Name);
                foreach (var ns in attributeType.GetNameSpaces().Compact())
                {
                    root = root.AddUsingNameSpace(ns);
                }
                rosType = rosType.AddAttribute(attributeType.Name, attribute.Properties.Select(x => (x.Name, x.Value)));
            }

            // Add members
            foreach (var member in type.Members.Compact())
            {
                (var codeMember, root) = member switch
                {
                    IField field => CreateRosField(root, field),
                    IMethod method => CreateRosMethod(root, method, type.Name),
                    IProperty prop => CreateRosProperty(root, prop),
                    _ => throw new NotImplementedException(),
                };
                rosType = rosType.AddMembers(codeMember);
            }
            rosNameSpace = rosNameSpace.AddType(rosType);
        }

        // Add additional namespaces to unit
        nameSpace.UsingNamespaces.ForEach(x => root = root.AddUsingNameSpace(x));

        // Add namespace to unit
        root = root!.AddNameSpace(rosNameSpace);

        var distinctUsings = root.DescendantNodes().OfType<UsingDirectiveSyntax>()
            .GroupBy(u => u.Name?.ToString()).Select(g => g.First())
            .Compact();
        var finalRoot = root.WithUsings(List(distinctUsings));

        return Result.Success(finalRoot.GenerateCode())!;
    }

    private static (MemberDeclarationSyntax Member, CompilationUnitSyntax Root) AddAttributes(PropertyDeclarationSyntax prop, CompilationUnitSyntax root, IMember member)
    {
        if (member.Attributes.Any())
        {
            foreach (var attribute in member.Attributes)
            {
                var attr = Attribute(attribute.Name.FullName);
                var attributeList = attr.ToList();
                prop = prop.AddAttributeLists(attributeList);

                attribute.Name.GetNameSpaces().ForEach(x => root = root.AddUsingNameSpace(x));
            }
        }
        return (prop, root);
    }

    private static (MemberDeclarationSyntax Member, CompilationUnitSyntax Root) CreateRosField(CompilationUnitSyntax root, IField member)
    {
        var modifiers = GeneratorHelper.ToModifiers(member.AccessModifier, member.InheritanceModifier);
        var result = CreateField(new(member.Name, member.Type, modifiers));
        member.Type.GetNameSpaces().ForEach(x => root = root.AddUsingNameSpace(x));
        return (result, root);
    }

    private static (MemberDeclarationSyntax Member, CompilationUnitSyntax Root) CreateRosMethod(CompilationUnitSyntax root, in IMethod method, in string className)
    {
        var modifiers = GeneratorHelper.ToModifiers(method.AccessModifier, method.InheritanceModifier);
        var result = method.IsConstructor || method.Name == className
            ? CreateConstructor(TypePath.GetName(className), modifiers, method.Arguments.Select(x => (x.Type, x.Name)), method.Body)
            : CreateMethod(new(method.Name, modifiers, method.IsAsync, method.ReturnType, method.Arguments.Select(x => (x.Type, x.Name)), method.Body, method.IsExtension));
        method.GetNameSpaces().ForEach(x => root = root.AddUsingNameSpace(x));

        foreach (var attribute in method.Attributes.Compact())
        {
            var attributeType = TypePath.New(attribute.Name);
            foreach (var ns in attributeType.GetNameSpaces().Compact())
            {
                root = root.AddUsingNameSpace(ns);
            }
            result = result.AddAttribute(attributeType.Name, attribute.Properties.Select(x => (x.Name, x.Value))).Cast().As<BaseMethodDeclarationSyntax>()!;
        }
        return (result, root);
    }

    private static (MemberDeclarationSyntax Member, CompilationUnitSyntax Root) CreateRosProperty(CompilationUnitSyntax root, IProperty member)
    {
        var propertyInfo = new RosPropertyInfo(member.Name, member.Type, (IEnumerable<SyntaxKind>?)GeneratorHelper.ToModifiers(member.AccessModifier, member.InheritanceModifier), (member.Getter is not null, GeneratorHelper.ToModifiers(member.Getter?.AccessModifier, null)), (member.Setter is not null, GeneratorHelper.ToModifiers(member.Setter?.AccessModifier, null)));
        var prop = member.BackingFieldName.IsNullOrEmpty()
            ? CreateProperty(propertyInfo)
            : CreatePropertyWithBackingField(propertyInfo, new RosFieldInfo(member.BackingFieldName, member.Type)).Property;
        member.Type.GetNameSpaces().ForEach(x => root = root.AddUsingNameSpace(x));
        var result = AddAttributes(prop, root, member);
        return result;
    }
}