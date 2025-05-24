namespace Library.CodeGenLib.Back;

public interface IMember : IValidatable, IHasAttributes
{
    AccessModifier AccessModifier { get; }
    InheritanceModifier InheritanceModifier { get; }
    string Name { get; }
}

public abstract class Member([DisallowNull] string name) : IMember
{
    public virtual AccessModifier AccessModifier { get; init; } = AccessModifier.Public;
    public ISet<ICodeGenAttribute> Attributes { get; } = new HashSet<ICodeGenAttribute>();
    public virtual InheritanceModifier InheritanceModifier { get; init; }
    public virtual string Name { get; } = name.ArgumentNotNull();

    public Result Validate() =>
        this.OnValidate();

    protected virtual Result OnValidate() =>
        Result.Succeed;
}