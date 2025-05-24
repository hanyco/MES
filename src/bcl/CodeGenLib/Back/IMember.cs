using Library.Validations;

namespace Library.CodeGenLib.Back;

public interface IMember : IValidatable, IHasAttributes
{
    AccessModifier AccessModifier { get; }
    InheritanceModifier InheritanceModifier { get; }
    string Name { get; }
}

public abstract class Member : IMember
{
    protected Member([DisallowNull] string name) =>
        this.Name = name.ArgumentNotNull();

    public virtual AccessModifier AccessModifier { get; init; } = AccessModifier.Public;
    public ISet<ICodeGenAttribute> Attributes { get; } = new HashSet<ICodeGenAttribute>();
    public virtual InheritanceModifier InheritanceModifier { get; init; }
    public virtual string Name { get; }

    public IResult Validate() =>
        this.OnValidate();

    protected virtual IResult OnValidate() =>
        IResult.Succeed;
}