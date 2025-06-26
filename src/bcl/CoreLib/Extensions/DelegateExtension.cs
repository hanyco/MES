namespace Library.Extensions;

public static class DelegateExtension
{
    extension(Action)
    {
        /// <summary>
        /// Catch the result of a function.
        /// </summary>
        /// <param name="tryMethod">      </param>
        /// <param name="catchMethod">    </param>
        /// <param name="finallyMethod">  </param>
        /// <param name="handling">       </param>
        /// <param name="throwException"> </param>
        /// <returns> </returns>
        public static Exception? Catch(
            Action tryMethod,
            Action<Exception>? catchMethod = null,
            Action? finallyMethod = null,
            bool throwException = false)
        {
            Check.MustBeArgumentNotNull(tryMethod);

            try
            {
                tryMethod();
                return null;
            }
            catch (Exception ex)
            {
                catchMethod?.Invoke(ex);
                if (throwException)
                {
                    throw;
                }

                return ex;
            }
            finally
            {
                finallyMethod?.Invoke();
            }
        }

        public static TResult Catch<TResult>(
            Func<TResult> tryMethod,
            Func<Exception, TResult> catchMethod,
            Action<TResult>? finallyMethod = null,
            bool throwException = false)
        {
            Check.MustBeArgumentNotNull(tryMethod);

            TResult output = default!;
            try
            {
                output = tryMethod();
            }
            catch (Exception ex)
            {
                Check.MustBe(catchMethod is not null || throwException);
                if (catchMethod is not null)
                {
                    output = catchMethod(ex);
                }

                if (throwException)
                {
                    throw;
                }
            }
            finally
            {
                finallyMethod?.Invoke(output);
            }
            return output;
        }

        public static (TResult? Result, Exception? Exception) Catch<TResult>(
            Func<TResult> tryMethod,
            TResult? defaultResult = default,
            Action<TResult>? finallyMethod = null,
            bool throwException = false)
        {
            Exception? exception = default;
            var result = Catch(tryMethod, ex =>
            {
                exception = ex;
                return defaultResult!;
            }, finallyMethod, throwException);
            return (result, exception);
        }

        /// <summary>
        /// Catch the result of a function and return the result value in <see cref="Result"/> class.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static async Task<Result> CatchAsync(Func<Task> action)
        {
            Check.MustBeArgumentNotNull(action);

            try
            {
                await action();
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Fail(error: ex);
            }
        }

        /// <summary>
        /// Catch the result of a function and return the result value in <see cref="Result"/> class.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="TException"></typeparam>
        /// <param name="action"></param>
        /// <param name="pickException"></param>
        /// <returns></returns>
        public static TResult? CatchFunc<TResult, TException>(in Func<TResult> action, in Func<TException, bool> pickException)
            where TException : Exception
        {
            try
            {
                return action.ArgumentNotNull()();
            }
            catch (TException ex) when (pickException.ArgumentNotNull()(ex))
            {
                return default;
            }
        }

        /// <summary>
        ///  Converts an action to an other action.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static Action ToAction(Action action)
            => action;

        /// <summary>
        /// Converts a function to an other function.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        public static Func<TResult> ToFunc<TResult>(Func<TResult> action)
            => action;

        /// <summary>
        /// Executes an action using a disposable object.
        /// </summary>
        /// <typeparam name="TDisposable"></typeparam>
        /// <param name="getItem"></param>
        /// <param name="action"></param>
        public static void Using<TDisposable>(Func<TDisposable> getItem, Action<TDisposable> action)
                    where TDisposable : IDisposable
        {
            using var item = getItem();
            action(item);
        }

        /// <summary>
        /// Executes an action using a disposable object.
        /// </summary>
        /// <typeparam name="TDisposable"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="getItem"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static TResult Using<TDisposable, TResult>(Func<TDisposable> getItem, Func<TDisposable, TResult> action)
            where TDisposable : IDisposable
        {
            using var item = getItem();
            return action(item);
        }
    }
}