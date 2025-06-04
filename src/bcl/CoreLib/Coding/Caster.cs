using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using Library.Validations;

namespace Library.Coding;

/// <summary>
/// Defines a contract for types that can be cast to an object. Provides a nullable property 'Value'
/// to retrieve the casted object.
/// </summary>
public interface ICastable
{
    object? Value { get; }
}

/// <summary>
/// An interface that provides a nullable property to retrieve a value of a specified type.
/// </summary>
/// <typeparam name="T">
/// The type of value that can be retrieved, allowing for flexibility in the data type used.
/// </typeparam>
public interface ICastable<T>
{
    T? Value { get; }
}

/// <summary>
/// Provides methods for casting objects to different types.
/// </summary>

[DebuggerStepThrough]
[StackTraceHidden]
public static class Caster
{
    /// <summary>
    /// Casts the value of the object to the specified type.
    /// </summary>
    /// <typeparam name="T"> The type to cast the value to. </typeparam>
    /// <param name="o"> The object. </param>
    /// <returns> The value of the object cast to the specified type. </returns>
    public static T? As<T>([DisallowNull] this ICastable o) where T : class =>
        o.Value as T;

    /// <summary>
    /// The entry of casting operations.
    /// </summary>
    /// <param name="obj"> The object to cast. </param>
    /// <returns> A new Castable object. </returns>
    public static ICastable Cast(this object? obj) =>
        new Castable(obj);

    public static ICastable<T> CastSafe<T>(this T? obj) =>
        new Castable<T>(obj);

    [return: NotNullIfNotNull(nameof(defaultValue))]
    public static T? Is<T>([DisallowNull] this ICastable o, T? defaultValue) where T : class =>
        o is T t ? t : defaultValue;

    /// <summary>
    /// Returns the result of a type match between the given object and the generic type T, or the
    /// default value of T if the match fails.
    /// </summary>
    public static T? Match<T>(object obj) =>
        obj is T result ? result : default;

    /// <summary>
    /// Returns a collection of objects of the specified type from the given collection.
    /// </summary>
    public static IEnumerable<T> OfType<T>(IEnumerable items) =>
        items.OfType<T>();

    public static ICastable<T> Safe<T>(this ICastable o, T? obj) =>
        new Castable<T>(obj);

    /// <summary>
    /// Casts the given object to the specified type.
    /// </summary>
    /// <typeparam name="T"> The type to cast the object to. </typeparam>
    /// <param name="o"> The object to cast. </param>
    /// <returns> The casted object. </returns>
    [return: NotNull]
    public static T To<T>([DisallowNull] this ICastable o) =>
        (T)o.Value!;

    public static TResult? To<T, TResult>([DisallowNull] this ICastable<T?> o, Func<T?, TResult?> converter) =>
        converter.ArgumentNotNull()(o.ArgumentNotNull().Value);

    public static byte ToByte([DisallowNull] this ICastable o, byte defaultValue = default, IFormatProvider? formatProvider = null)
    {
        //Check if the value of o is an integer
        if (o.Value is byte intValue)
        {
            //If it is, return the integer value
            return intValue;
        }

        //Check if the value of o is IConvertible
        if (o.Value is IConvertible convertible)
        {
            //If it is, convert it to an integer using the format provider
            return convertible.ToByte(formatProvider);
        }

        //Try to parse the value of o as an integer
        if (!byte.TryParse(Convert.ToString(o.Value, formatProvider), out var result))
        {
            //If it fails, set the result to the default value
            result = defaultValue;
        }

        //Return the result
        return result;
    }

    /// <summary>
    /// Converts the specified object to an integer.
    /// </summary>
    /// <param name="o">              The object to convert. </param>
    /// <param name="defaultValue">   The default value to use if the conversion fails. </param>
    /// <param name="formatProvider"> The format provider to use for the conversion. </param>
    /// <returns> The converted integer. </returns>
    public static int ToInt([DisallowNull] this ICastable o, int defaultValue = default, IFormatProvider? formatProvider = null)
    {
        //Check if the value of o is an integer
        if (o.Value is int intValue)
        {
            //If it is, return the integer value
            return intValue;
        }

        //Check if the value of o is IConvertible
        if (o.Value is IConvertible convertible)
        {
            //If it is, convert it to an integer using the format provider
            return convertible.ToInt32(formatProvider);
        }

        //Try to parse the value of o as an integer
        if (!int.TryParse(Convert.ToString(o.Value, formatProvider), out var result))
        {
            //If it fails, set the result to the default value
            result = defaultValue;
        }

        //Return the result
        return result;
    }

    /// <summary>
    /// Converts the value of the specified object to a long.
    /// </summary>
    /// <param name="o"> The object to convert. </param>
    /// <returns> A long that represents the value of the specified object. </returns>
    public static long ToLong([DisallowNull] this ICastable o) =>
        Convert.ToInt64(o.Value);

    /// <summary>
    /// Filters a sequence of items to return only those of type T.
    /// </summary>
    /// <typeparam name="T"> The type of items to return. </typeparam>
    /// <param name="items"> The sequence of items to filter. </param>
    /// <returns> An <see cref="IEnumerableT" /> containing only those items of type T. </returns>
    public static IEnumerable<T> TypeOf<T>(IEnumerable items)
    {
        if (items is null)
        {
            yield break;
        }
        foreach (var item in items)
        {
            if (TypeOf<T>(item) is { } result)
            {
                yield return result;
            }
        }
    }

    /// <summary>
    /// Returns the specified type of the given object, or the default value if the object is not of
    /// the specified type.
    /// </summary>
    public static T? TypeOf<T>(object obj) =>
        obj?.GetType() == typeof(T) ? (T)obj : default;

    /// <summary>
    /// Filters a sequence of items and returns only those of type T.
    /// </summary>
    /// <param name="items"> The sequence of items to filter. </param>
    /// <returns> An IEnumerable of type T containing the filtered items. </returns>
    public static IEnumerable<T> WhereIs<T>(IEnumerable items)
    {
        if (items is null)
        {
            yield break;
        }
        foreach (var item in items)
        {
            if (item is T result)
            {
                yield return result;
            }
        }
    }
}

internal readonly record struct Castable(object? Value) : ICastable { }
internal readonly record struct Castable<T>(T? Value) : ICastable<T> { }