using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using Library.Casting;
using Library.Extensions;
using Library.Validations;

namespace Library.Extensions;

/// <summary>
/// Provides methods for casting objects to different types.
/// </summary>
[DebuggerStepThrough]
[StackTraceHidden]
public static class CastingExtensions
{
    extension([DisallowNull] ICastable o)
    {
        /// <summary>
        /// Casts the value of the object to the specified type.
        /// </summary>
        /// <typeparam name="T">The type to cast the value to.</typeparam>
        /// <param name="o">The object.</param>
        /// <returns>The value of the object cast to the specified type.</returns>
        public T? As<T>() where T : class =>
            o.Value as T;

        [return: NotNullIfNotNull(nameof(defaultValue))]
        public T? Is<T>(T? defaultValue) where T : class =>
            o is T t ? t : defaultValue;
        /// <summary>
        /// Casts the given object to the specified type.
        /// </summary>
        /// <typeparam name="T">The type to cast the object to.</typeparam>
        /// <param name="o">The object to cast.</param>
        /// <returns>The casted object.</returns>
        [return: NotNull]
        public T To<T>() =>
            (T)o.Value!;

        public byte ToByte(byte defaultValue = default, IFormatProvider? formatProvider = null)
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
        /// <param name="o">The object to convert.</param>
        /// <param name="defaultValue">The default value to use if the conversion fails.</param>
        /// <param name="formatProvider">The format provider to use for the conversion.</param>
        /// <returns>The converted integer.</returns>
        public int ToInt(int defaultValue = default, IFormatProvider? formatProvider = null)
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
        /// <param name="o">The object to convert.</param>
        /// <returns>A long that represents the value of the specified object.</returns>
        public long ToLong() =>
            Convert.ToInt64(o.Value);
    }
    extension<T>([DisallowNull] ICastable<T> o)
    {
        public TResult? To<TResult>(Func<T, TResult?> converter) =>
            converter.ArgumentNotNull()(o.ArgumentNotNull().Value);
    }

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
    /// <summary>
    /// Filters a sequence of items to return only those of type T.
    /// </summary>
    /// <typeparam name="T">The type of items to return.</typeparam>
    /// <param name="items">The sequence of items to filter.</param>
    /// <returns>An <see cref="IEnumerableT"/> containing only those items of type T.</returns>
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
    /// <param name="items">The sequence of items to filter.</param>
    /// <returns>An IEnumerable of type T containing the filtered items.</returns>
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