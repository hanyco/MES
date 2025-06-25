using System.Reflection;

using Library.DesignPatterns.Creational.Exceptions;

namespace Library.DesignPatterns.Creational;

/// <summary>
/// A Singleton using an StaticAllocator used just to simplify the inheritance syntax.
/// </summary>
public class Singleton<T> : ISingleton<T>
    where T : class, ISingleton<T>
{
    private static readonly Lazy<T> _instance = GenerateLazySingletonInstance(initializeInstance: InitializeInstance);

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public static T Instance => _instance.Value;

    /// <summary>
    /// The instance initializer
    /// </summary>
    protected static Action<T>? InitializeInstance { get; set; }

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
        var ci = createInstance ?? object.GetMethod<Func<TSingleton>>(typeof(TSingleton),
            "CreateInstance",
            BindingFlags.Public | BindingFlags.NonPublic |
            BindingFlags.Static);
        var result = ci?.Invoke();

        //! if not, try to find a non-public constructor instead. A non-public constructor is mandatory.
        if (result is null)
        {
            var constructor = typeof(TSingleton).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, [], [])
                ?? throw new SingletonException($"""The class must have a static method: "{typeof(TSingleton)} CreateInstance()" or a private/protected parameter-less constructor.""");
            result = constructor.Invoke([]) as TSingleton;

            //! Just to make sure that the code will work.
            if (result is null)
            {
                throw new SingletonException("Couldn't create instance.");
            }
        }

        //! If (T) has implemented CreateInstance as an instantiate method, use it to initialize the instance.
        var initialize = object.GetMethod<Action>(result, "InitializeComponents");
        initialize?.Invoke();

        //! if(T) has initialized b"InitializeInstance" delegate, call it to initialize the instance.
        initializeInstance?.Invoke(result);
        return result;
    }
}