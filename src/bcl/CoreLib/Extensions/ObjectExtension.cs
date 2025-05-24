using Library.Casting;

namespace Library.Extensions;

public static class ObjectExtension
{
    /// <summary>
    /// The entry of casting operations.
    /// </summary>
    /// <param name="obj">The object to cast.</param>
    /// <returns>A new Castable object.</returns>
    public static ICastable Cast(this object? obj) =>
        new Castable(obj);

    public static ICastable<T> CastSafe<T>(this T? obj) =>
        new Castable<T>(obj);
}