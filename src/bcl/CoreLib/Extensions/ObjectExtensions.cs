using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using Library.Casting;
using Library.Validations;

namespace Library.Extensions;

[DebuggerStepThrough]
[StackTraceHidden]
public static class ObjectExtensions
{
    extension([DisallowNull] object? obj)
    {
        /// <summary>
        /// Creates a new instance of <see cref="ICastable"/> wrapping the current object.
        /// </summary>
        /// <returns>A castable wrapper for the object.</returns>
        [return: NotNull]
        public ICastable Cast() => new Castable(obj);
    }

    extension<T>([DisallowNull] T? obj)
    {
        /// <summary>
        /// Safely wraps the object in an <see cref="ICastable{T}"/> to enable generic conversions.
        /// </summary>
        /// <returns>A castable wrapper of type <typeparamref name="T"/>.</returns>
        [return: NotNull]
        public ICastable<T> CastSafe() => new Castable<T>(obj);
    }

    /// <summary>
    /// Gets the attribute.
    /// </summary>
    /// <typeparam name="TAttribute"> The type of the attribute. </typeparam>
    /// <param name="value">        The value. </param>
    /// <param name="defaultValue"> The default value. </param>
    /// <param name="inherited">    if set to <c> true </c> [inherited]. </param>
    /// <returns> </returns>
    public static TAttribute? GetAttribute<TAttribute>(in object value,
        in TAttribute? defaultValue = null,
        bool inherited = true)
        where TAttribute : Attribute
    {
        var attributes = value.GetType().GetCustomAttributes(typeof(TAttribute), inherited);
        return attributes.Length > 0 ? (TAttribute)attributes[0] : defaultValue;
    }

    /// <summary>
    /// Gets the specified attribute from the given property.
    /// </summary>
    /// <typeparam name="TAttribute"> The type of the attribute. </typeparam>
    /// <param name="property"> The property. </param>
    /// <returns> The attribute. </returns>
    public static TAttribute? GetAttribute<TAttribute>(in PropertyInfo property)
        where TAttribute : Attribute =>
        property is null
                ? throw new ArgumentNullException(nameof(property))
                : property.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault().Cast().As<TAttribute>();

    /// <summary>
    /// Gets the attribute.
    /// </summary>
    /// <typeparam name="TAttribute"> The type of the attribute. </typeparam>
    /// <param name="type">         The type. </param>
    /// <param name="defaultValue"> The default value. </param>
    /// <param name="inherited">    if set to <c> true </c> [inherited]. </param>
    /// <returns> </returns>
    public static TAttribute? GetAttribute<TAttribute>(in Type type,
        in TAttribute? defaultValue = null,
        bool inherited = true)
        where TAttribute : Attribute
    {
        var attributes = type.GetCustomAttributes(typeof(TAttribute), inherited);
        return attributes.Length > 0 ? (TAttribute)attributes[0] : defaultValue;
    }

    /// <summary> Gets the value of a property of a given object. </summary> <typeparam
    /// name="TPropertyType">The type of the property.</typeparam> <param name="obj">The
    /// object.</param> <param name="propName">The name of the property.</param> <param
    /// name="searchPrivates">Whether to search
    public static TPropertyType? GetProp<TPropertyType>([DisallowNull] in object obj, [DisallowNull] string propName, bool searchPrivates = false)
    {
        var type = obj.ArgumentNotNull().GetType();
        var properties = type.GetProperties();
        if (properties.Length == 0)
        {
            properties = type.GetProperties(searchPrivates
                ? BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic
                : BindingFlags.Default);
        }

        var property = properties.FirstOrDefault(prop => string.Equals(prop.Name, propName, StringComparison.Ordinal));
        return property is not null ? (TPropertyType?)property.GetValue(obj, null) : default;
    }

    /// <summary>
    /// Determines whether an instance of a specified type can be assigned to a variable of the
    /// current type.
    /// </summary>
    /// <param name="obj">  The object. </param>
    /// <param name="type"> The type. </param>
    /// <returns>
    /// <c> true </c> if [is inherited or implemented] [the specified object]; otherwise, <c> false </c>.
    /// </returns>
    public static bool IsInheritedOrImplemented(in object? obj, [DisallowNull] in Type type)
        => obj != null && type.ArgumentNotNull().IsAssignableFrom(obj.GetType());
}