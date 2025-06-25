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
    }

    extension<TArg>(Action<TArg>)
    {
    }

    extension<TOutput>(Func<TOutput>)
    {
        public TOutput Catch(
            Func<TOutput>? tryMethod,
            Func<Exception, TOutput> catchMethod,
            Action<TOutput>? finallyMethod = null,
            bool throwException = false)
        {
            Check.MustBeArgumentNotNull(tryMethod);

            TOutput output = default!;
            try
            {
                output = tryMethod();
            }
            catch (Exception ex)
            {
                output = catchMethod(ex);
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
    }
}