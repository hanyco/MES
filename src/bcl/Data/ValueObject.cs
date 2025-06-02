using System.Collections;
using System.Diagnostics.CodeAnalysis;

using Library.Validations;

namespace Library.Data;

public abstract record ValueObjectBase<TValueObject> : IStructuralEquatable
    where TValueObject : ValueObjectBase<TValueObject>
{
    public virtual bool Equals(TValueObject? other) =>
        other is { } o && o.GetHashCode() == this.GetHashCode();

    public virtual bool Equals(object? other, IEqualityComparer comparer) =>
        comparer?.Equals(this, other) ?? false;

    public virtual int GetHashCode(IEqualityComparer comparer) =>
        comparer.ArgumentNotNull().GetHashCode(this);

    public override int GetHashCode() =>
        this.OnGetHashCode();

    protected abstract int OnGetHashCode();
}

public abstract record ValueObjectBase<TValue, TValueObject>(TValue value) : ValueObjectBase<ValueObjectBase<TValue, TValueObject>>
    where TValueObject : ValueObjectBase<TValue, TValueObject>
{
    protected override int OnGetHashCode() =>
        this.value?.GetHashCode() ?? default;
}

public sealed record ValueObject<TValue>(TValue value) : ValueObjectBase<TValue, ValueObject<TValue>>(value)
{
    [return: NotNullIfNotNull(nameof(o.value))]
    public static implicit operator TValue(in ValueObject<TValue> o)
        => o.ArgumentNotNull().value;

    [return: NotNull]
    public static implicit operator ValueObject<TValue>(in TValue o)
        => new(o);
}