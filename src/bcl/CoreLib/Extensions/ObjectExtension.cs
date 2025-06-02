using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using Library.Casting;

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
}