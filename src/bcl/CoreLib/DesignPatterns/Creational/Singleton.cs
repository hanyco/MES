namespace Library.DesignPatterns.Creational;

/// <summary>
/// A Singleton using an StaticAllocator used just to simplify the inheritance syntax.
/// </summary>
public class Singleton<T> : ISingleton<T>
    where T : class, ISingleton<T>
{
    private static readonly Lazy<T> _instance = ObjectHelper.GenerateLazySingletonInstance(initializeInstance: InitializeInstance);

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public static T Instance => _instance.Value;

    /// <summary>
    /// The instance initializer
    /// </summary>
    protected static Action<T>? InitializeInstance { get; set; }
}