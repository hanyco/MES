namespace Library.DesignPatterns.Creational;

/// <summary>
/// Generic Singleton Interface
/// </summary>
/// <typeparam name="TSingleton">The type of the singleton.</typeparam>
public interface ISingleton<TSingleton>
    where TSingleton : class, ISingleton<TSingleton>
{
    public static abstract TSingleton Instance { get; }
}