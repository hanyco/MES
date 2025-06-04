using Library.DesignPatterns.Markers;

namespace Library.DesignPatterns;

[Immutable]
public sealed class Monad<TValue>
{
    private readonly TValue? _value;

    public Monad(TValue? value)
        => this._value = value;

    public Monad<TResult?> AnyWay<TResult>(in Func<TValue?, TResult?> func)
        => new(func(this._value));

    public Monad<TValue?> AnyWay(in Action<TValue?> func)
    {
        func(this._value);
        return this;
    }

    public Monad<TResult?> NotNull<TResult>(in Func<TValue, TResult?> func)
        => this._value is not null ? new(func(this._value)) : new(default);

    public Monad<TValue?> NotNull(in Action<TValue> func)
    {
        if (this._value is not null)
        {
            func(this._value);
        }

        return this;
    }

    public async Task<Monad<TValue?>> NotNullAsync(Func<TValue, Task<TValue>> func) 
        => this._value is not null ? (new(await func(this._value))) : (new(default));

    public async Task<Monad<TValue?>> NotNullAsync(Func<TValue, Task> func)
    {
        if (this._value is not null)
        {
            await func(this._value);
        }

        return this;
    }
}