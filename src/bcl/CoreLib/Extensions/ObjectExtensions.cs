using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using Library.DesignPatterns.Creational;
using Library.DesignPatterns.Creational.Exceptions;
using Library.Validations;

namespace Library.Extensions;

[DebuggerStepThrough]
[StackTraceHidden]
public static class ObjectExtensions
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

    /// <summary>
    /// Generates the lazy singleton instance.
    /// </summary>
    /// <typeparam name="TSingleton">The type of the singleton.</typeparam>
    /// <param name="createInstance">The create instance.</param>
    /// <param name="initializeInstance">The initialize instance.</param>
    /// <returns></returns>
    public static Lazy<TSingleton> GenerateLazySingletonInstance<TSingleton>(
        Func<TSingleton>? createInstance = null,
        Action<TSingleton>? initializeInstance = null)
        where TSingleton : class, ISingleton<TSingleton>
        => new(() => GenerateSingletonInstance<TSingleton>());

    /// <summary>
    /// Generates a singleton instance of a class (Must be cached by the owner class).
    /// </summary>
    /// <typeparam name="TSingleton">The type of class which is being instantiated.</typeparam>
    /// <param name="createInstance">A delegate to be used for getting an instance of</param>
    /// <param name="initializeInstance">The instance initializer.</param>
    /// <returns></returns>
    /// <exception cref="SingletonException">
    /// The class must have a static method: "TSingleton CreateInstance()" or a private/protected constructor.
    /// </exception>
    /// <remarks>
    /// Before generating instance, GenerateSingletonInstance searches for a static method:
    /// "TSingleton CreateInstance()". If found, GenerateSingletonInstance calls it and retrieves an
    /// instance. Otherwise, a private/protected parameter-less constructor is required to construct
    /// an instance of TSingleton./&gt; After generating instance, searches for a method named:
    /// "InitializeComponents". If found will be called.
    /// </remarks>
    [return: NotNull]
    public static TSingleton GenerateSingletonInstance<TSingleton>(Func<TSingleton>? createInstance = null, Action<TSingleton>? initializeInstance = null)
        where TSingleton : class, ISingleton<TSingleton>
    {
        //! If (T) has implemented CreateInstance as a static method, use it to create an instance
        var ci = createInstance ?? GetMethod<Func<TSingleton>>(typeof(TSingleton),
            "CreateInstance",
            BindingFlags.Public | BindingFlags.NonPublic |
            BindingFlags.Static);
        var result = ci?.Invoke();

        //! if not, try to find a non-public constructor instead. A non-public constructor is mandatory.
        if (result is null)
        {
            var constructor = typeof(TSingleton).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                EnumerableExtension.EmptyArray<Type>(),
                EnumerableExtension.EmptyArray<ParameterModifier>()) ?? throw new SingletonException($"""The class must have a static method: "{typeof(TSingleton)} CreateInstance()" or a private/protected parameter-less constructor.""");
            result = constructor.Invoke(EnumerableExtension.EmptyArray<object>()) as TSingleton;

            //! Just to make sure that the code will work.
            if (result is null)
            {
                throw new SingletonException("Couldn't create instance.");
            }
        }

        //! If (T) has implemented CreateInstance as an instantiate method, use it to initialize the instance.
        var initialize = GetMethod<Action>(result, "InitializeComponents");
        initialize?.Invoke();

        //! if(T) has initialized b"InitializeInstance" delegate, call it to initialize the instance.
        initializeInstance?.Invoke(result);
        return result;
    }
    /// <summary>
    /// Gets the method.
    /// </summary>
    /// <typeparam name="TDelegate">The type of the delegate.</typeparam>
    /// <param name="objType">Type of the object.</param>
    /// <param name="name">The name.</param>
    /// <param name="bindingFlags">The binding flags.</param>
    /// <returns></returns>
    public static TDelegate? GetMethod<TDelegate>(in Type objType,
        in string name,
        in BindingFlags bindingFlags = BindingFlags.Public)
        where TDelegate : class
    {
        var methodInfo = objType.GetMethod(name, bindingFlags);
        return methodInfo is not null
            ? Delegate.CreateDelegate(typeof(TDelegate), null, methodInfo).Cast().As<TDelegate>()
            : null;
    }

    /// <summary>
    /// Gets a method from an object using the specified name and binding flags.
    /// </summary>
    /// <typeparam name="TDelegate">The type of the delegate.</typeparam>
    /// <param name="obj">The object.</param>
    /// <param name="name">The name of the method.</param>
    /// <param name="bindingFlags">The binding flags.</param>
    /// <returns>The delegate for the method, or null if the method was not found.</returns>
    public static TDelegate? GetMethod<TDelegate>([DisallowNull] in object obj,
        [DisallowNull] in string name,
        BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        where TDelegate : class
    {
        var methodInfo = obj.ArgumentNotNull().GetType().GetMethod(name, bindingFlags);
        return methodInfo is not null
            ? Delegate.CreateDelegate(typeof(TDelegate), obj, methodInfo).Cast().As<TDelegate>()
            : null;
    }
}