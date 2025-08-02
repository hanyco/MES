using System.Reflection;

using Library.Coding;

namespace Library.Extensions;

public static class ReflectionHelper
{
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
    /// Gets a method from an object using the specified name and binding flags.
    /// </summary>
    /// <typeparam name="TDelegate"> The type of the delegate. </typeparam>
    /// <param name="obj">          The object. </param>
    /// <param name="name">         The name of the method. </param>
    /// <param name="bindingFlags"> The binding flags. </param>
    /// <returns> The delegate for the method, or null if the method was not found. </returns>
    public static TDelegate? GetMethod<TDelegate>([DisallowNull] object obj,
        [DisallowNull] in string name,
        BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        where TDelegate : class
    {
        var methodInfo = obj.EnsureArgumentNotNull().GetType().GetMethod(name, bindingFlags);
        return methodInfo is not null
            ? Delegate.CreateDelegate(typeof(TDelegate), obj, methodInfo).Cast().As<TDelegate>()
            : null;
    }

    /// <summary> Gets the value of a property of a given object. </summary> <typeparam
    /// name="TPropertyType">The type of the property.</typeparam> <param name="obj">The
    /// object.</param> <param name="propName">The name of the property.</param> <param
    /// name="searchPrivates">Whether to search
    public static TPropertyType? GetPropertyValue<TPropertyType>([DisallowNull] in object obj, [DisallowNull] string propName, bool searchPrivates = false)
    {
        var type = obj.EnsureArgumentNotNull().GetType();
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
        => obj != null && type.EnsureArgumentNotNull().IsAssignableFrom(obj.GetType());
}