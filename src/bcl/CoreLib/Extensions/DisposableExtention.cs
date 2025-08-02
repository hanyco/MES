namespace Library.Extensions;

public static class DisposableExtention
{
    extension(IDisposable)
    {
        /// <summary>
        /// Disposes the specified disposable object.
        /// </summary>
        /// <typeparam name="TDisposable"> The type of the disposable. </typeparam>
        /// <param name="disposable"> The disposable. </param>
        /// <param name="action">     The action. </param>
        public static void Dispose<TDisposable>(in TDisposable disposable, in Action<TDisposable>? action = null) where TDisposable : IDisposable
            => Dispose(disposable, action);

        /// <summary>
        /// Disposes the specified disposable object.
        /// </summary>
        /// <typeparam name="TDisposable"> The type of the disposable. </typeparam>
        /// <typeparam name="TResult"> The type of the result. </typeparam>
        /// <param name="disposable"> The disposable. </param>
        /// <param name="action">     The action. </param>
        /// <returns> </returns>
        public static TResult Dispose<TDisposable, TResult>(in TDisposable disposable, in Func<TDisposable, TResult> action) where TDisposable : IDisposable
            => Dispose(disposable, action);

        /// <summary>
        /// Disposes the specified disposable object.
        /// </summary>
        /// <typeparam name="TDisposable"> The type of the disposable. </typeparam>
        /// <typeparam name="TResult"> The type of the result. </typeparam>
        /// <param name="disposable"> The disposable. </param>
        /// <param name="result">     The result. </param>
        /// <returns> </returns>
        public static TResult Dispose<TDisposable, TResult>(in TDisposable disposable, in TResult result) where TDisposable : IDisposable
            => Dispose(disposable, result);

        /// <summary>
        /// Disposes the specified disposable object.
        /// </summary>
        /// <typeparam name="TDisposable"> The type of the disposable. </typeparam>
        /// <typeparam name="TResult"> The type of the result. </typeparam>
        /// <param name="disposable"> The disposable. </param>
        /// <param name="action">     The action. </param>
        /// <returns> </returns>
        public static TResult Dispose<TDisposable, TResult>(in TDisposable disposable, in Func<TResult> action) where TDisposable : IDisposable
            => Dispose(disposable, action);
    }

    extension(IDisposable @this)
    {
    }

    extension<TDisposable>(TDisposable @this)
        where TDisposable : IDisposable
    {
        /// <summary>
        /// Disposes the specified disposable object.
        /// </summary>
        /// <typeparam name="TDisposable"> The type of the disposable. </typeparam>
        /// <param name="this">   The disposable. </param>
        /// <param name="action"> The action. </param>
        public void Dispose(in Action<TDisposable>? action = null)
        {
            try
            {
                action?.Invoke(@this);
            }
            finally
            {
                @this?.Dispose();
            }
        }

        /// <summary>
        /// Disposes the specified disposable object.
        /// </summary>
        /// <typeparam name="TDisposable"> The type of the disposable. </typeparam>
        /// <typeparam name="TResult"> The type of the result. </typeparam>
        /// <param name="this">   The disposable. </param>
        /// <param name="action"> The action. </param>
        /// <returns> </returns>
        public TResult Dispose<TResult>(in Func<TDisposable, TResult> action)
        {
            Check.MustBeArgumentNotNull(action);
            try
            {
                return action(@this);
            }
            finally
            {
                @this?.Dispose();
            }
        }

        /// <summary>
        /// Disposes the specified disposable object.
        /// </summary>
        /// <typeparam name="TDisposable"> The type of the disposable. </typeparam>
        /// <typeparam name="TResult"> The type of the result. </typeparam>
        /// <param name="this">   The disposable. </param>
        /// <param name="result"> The result. </param>
        /// <returns> </returns>
        public TResult Dispose<TResult>(in TResult result)
        {
            try
            {
                return result;
            }
            finally
            {
                @this?.Dispose();
            }
        }

        /// <summary>
        /// Disposes the specified disposable object.
        /// </summary>
        /// <typeparam name="TDisposable"> The type of the disposable. </typeparam>
        /// <typeparam name="TResult"> The type of the result. </typeparam>
        /// <param name="this">   The disposable. </param>
        /// <param name="action"> The action. </param>
        /// <returns> </returns>
        public TResult Dispose<TResult>(in Func<TResult> action)
        {
            Check.MustBeArgumentNotNull(action);
            try
            {
                return action();
            }
            finally
            {
                @this?.Dispose();
            }
        }
    }
}