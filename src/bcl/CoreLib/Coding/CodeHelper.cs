using System.Diagnostics;
using System.Reflection;

using Library.DesignPatterns.ExceptionHandlingPattern;
using Library.Exceptions;
using Library.Resulting;
using Library.Validations;

namespace Library.Coding;

/// <summary>
/// 🤏 Little code snippets to do little works.
/// </summary>
[DebuggerStepThrough, StackTraceHidden]
public static class CodeHelper
{
    /// <summary>
    /// Breaks code execution by throwing a <see cref="BreakException"/>.
    /// </summary>
    [DoesNotReturn]
    public static void Break() =>
        BreakException.Throw();

    /// <summary>
    /// Breaks code execution by throwing a <see cref="BreakException"/>.
    /// </summary>
    [DoesNotReturn]
    public static T Break<T>() =>
        throw new BreakException();

    /// <summary>
    /// Catch the result of a function.
    /// </summary>
    /// <param name="tryMethod"></param>
    /// <param name="catchMethod"></param>
    /// <param name="finallyMethod"></param>
    /// <param name="handling"></param>
    /// <param name="throwException"></param>
    /// <returns></returns>
    public static Exception? Catch(
        in Action tryMethod,
        in Action<Exception>? catchMethod = null,
        in Action? finallyMethod = null,
        in ExceptionHandling? handling = null,
        in bool throwException = false)
    {
        Check.MustBeArgumentNotNull(tryMethod);

        handling?.Reset();
        try
        {
            tryMethod();
            return null;
        }
        catch (Exception ex)
        {
            catchMethod?.Invoke(ex);
            handling?.HandleException(ex);
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

    /// <summary>
    /// Catch the result of a function.
    /// </summary>
    /// <param name="action"></param>
    /// <param name="exceptionHandling"></param>
    public static void Catch(in Action action, in ExceptionHandling exceptionHandling)
        => Catch(action, handling: exceptionHandling);

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
    /// <param name="action"></param>
    /// <returns></returns>
    public static async Task<Result<TResult?>> CatchAsync<TResult>(Func<Task<TResult?>> action)
    {
        try
        {
            return Result.Success<TResult?>(await action());
        }
        catch (Exception ex)
        {
            return Result.Fail<TResult?>(error: ex);
        }
    }

    /// <summary>
    /// Catch the result of a function and return the result value and the exception if occurred.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="action"></param>
    /// <returns></returns>
    public static (TResult? Result, Exception? Exception) CatchFunc<TResult>(in Func<TResult> action)
    {
        Check.MustBeArgumentNotNull(action);
        try
        {
            return (action(), null);
        }
        catch (Exception ex)
        {
            return (default, ex);
        }
    }

    /// <summary>
    /// Catch the result of a function and return the result value and the exception if occurred.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="action"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static (TResult Result, Exception? Exception) CatchFunc<TResult>(in Func<TResult> action, in TResult defaultValue)
    {
        Check.MustBeArgumentNotNull(action);
        try
        {
            return (action(), null);
        }
        catch (Exception ex)
        {
            return (defaultValue, ex);
        }
    }

    /// <summary>
    /// Catch the result of a function and return the result value in <see cref="Result"/> class.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <typeparam name="TException"></typeparam>
    /// <param name="action"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static TResult? CatchFunc<TResult, TException>(in Func<TResult> action, in Predicate<TException> predicate)
        where TException : Exception
    {
        try
        {
            return action.ArgumentNotNull()();
        }
        catch (TException ex) when (predicate.ArgumentNotNull()(ex))
        {
            return default;
        }
    }

    /// <summary>
    /// Catch the result of a function and return the result value in <see cref="Result"/> class.
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public static Result CatchResult([DisallowNull] in Action action)
    {
        Check.MustBeArgumentNotNull(action);
        try
        {
            action();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.GetBaseException().Message, ex);
        }
    }

    /// <summary>
    /// Catch the result of a function and return the result value in <see cref="Result"/> class.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="action"></param>
    /// <param name="onErrorResult"></param>
    /// <returns></returns>
    public static Result<TResult?> CatchResult<TResult>([DisallowNull] in Func<TResult> action, TResult? onErrorResult = default)
    {
        Check.MustBeArgumentNotNull(action);
        try
        {
            return Result.Success(action());
        }
        catch (Exception ex)
        {
            return Result.Fail<TResult?>(onErrorResult, ex.GetBaseException().Message, ex)!;
        }
    }

    /// <summary>
    /// Catch the result of a function and return the result value in <see cref="Result"/> class.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <typeparam name="TArg"></typeparam>
    /// <param name="method"></param>
    /// <param name="arg"></param>
    /// <returns></returns>
    public static Result<IEnumerable<TResult?>?> CatchResult<TResult, TArg>(Func<TArg, IEnumerable<TResult?>> method, TArg arg)
    {
        Check.MustBeArgumentNotNull(method);
        try
        {
            var result = method(arg);
            return Result.Success(result);
        }
        catch (Exception ex)
        {
            return Result.Fail<IEnumerable<TResult?>>(ex);
        }
    }

    /// <summary>
    /// Catch the result of a function and return the result value in <see cref="Result"/> class.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="func"></param>
    /// <param name="defaultResult"></param>
    /// <returns></returns>
    public static async Task<Result<TResult>> CatchResult<TResult>(Func<Task<TResult>> func, TResult? defaultResult = default)
    {
        Check.MustBeArgumentNotNull(func);
        try
        {
            var result = await func();
            return Result.Success<TResult>(result);
        }
        catch (Exception ex)
        {
            return Result.Fail<TResult>(defaultResult, ex);
        }
    }

    /// <summary>
    /// Catch the result of a function and return the result value in <see cref="Result"/> class.
    /// </summary>
    /// <param name="func"></param>
    /// <returns></returns>
    public static async Task<Result> CatchResultAsync(Func<Task<Result>> func)
    {
        Check.MustBeArgumentNotNull(func);
        try
        {
            return await func();
        }
        catch (Exception ex)
        {
            return Result.Fail(ex);
        }
    }

    /// <summary>
    /// Catch the result of a function and return the result value in <see cref="Result"/> class.
    /// </summary>
    /// <param name="func"></param>
    /// <returns></returns>
    public static async Task<Result> CatchResultAsync(Func<Task> func)
    {
        Check.MustBeArgumentNotNull(func);
        try
        {
            await func();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Fail(ex);
        }
    }

    public static async Task<Result<TValue>> CatchResultAsync<TValue>(Func<Task<TValue>> func)
    {
        Check.MustBeArgumentNotNull(func);
        try
        {
            var value = await func();
            return Result.Success(value);
        }
        catch (Exception ex)
        {
            return Result.Fail<TValue>(ex);
        }
    }

    /// <summary>
    /// Compose a function with a given argument.
    /// </summary>
    /// <typeparam name="TArgs"></typeparam>
    /// <typeparam name="TResult2"></typeparam>
    /// <param name="args"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    public static Func<TResult2> Compose<TArgs, TResult2>(TArgs args, Func<TArgs, TResult2> func)
    {
        Check.MustBeArgumentNotNull(func);
        return [DebuggerStepThrough] () => func(create());
        TArgs create() => args;
    }

    public static Func<TResult2> Compose<TArgs, TResult2>([DisallowNull] this Func<TArgs> create, Func<TArgs, TResult2> func)
    {
        Check.MustBeArgumentNotNull(create);
        Check.MustBeArgumentNotNull(func);
        return [DebuggerStepThrough] () => func(create());
    }

    /// <summary> Compose a Func<Result<TResult1>> and a Func<TResult1, Result<TResult2>> into a
    /// Func<Result<TResult2>>. </summary> <typeparam name="TArgs">The type of the first
    /// result.</typeparam> <typeparam name="TResult2">The type of the second result.</typeparam>
    /// <param name="create">The Func<Result<TResult1>> to compose.</param> <param name="func">The
    /// Func<TResult1, Result<TResult2>> to compose.</param> <param name="onFail">The
    /// Func<Result<TResult1>, Result<TResult2>> to invoke if the first result is a failure.</param>
    /// <returns>A Func<Result<TResult2>> composed of the two functions.</returns>
    public static Func<Result<TResult2>> Compose<TArgs, TResult2>([DisallowNull] this Func<Result<TArgs>> create, Func<TArgs, Result<TResult2>> func, Func<Result<TArgs>, Result<TResult2>>? onFail = null)
    {
        Check.MustBeArgumentNotNull(create);
        Check.MustBeArgumentNotNull(func);
        return [DebuggerStepThrough] () =>
        {
            var result = create();
            return result.IsSucceed
                ? func(result.Value)
                : onFail?.Invoke(result) ?? Result.From<TResult2>(result, default!);
        };
    }

    /// <summary>
    /// Compose a function with a given argument.
    /// </summary>
    /// <typeparam name="TArgs">The type of the result of the first function.</typeparam>
    /// <typeparam name="TResult2">The type of the result of the composed function.</typeparam>
    /// <typeparam name="TArg">The type of the argument of the composed function.</typeparam>
    /// <param name="create">The first function.</param>
    /// <param name="func">The composed function.</param>
    /// <param name="arg">The argument of the composed function.</param>
    /// <returns>The composed function.</returns>
    public static Func<TResult2> Compose<TArgs, TResult2, TArg>([DisallowNull] this Func<TArgs> create, Func<TArgs, TArg, TResult2> func, TArg arg)
    {
        Check.MustBeArgumentNotNull(create);
        Check.MustBeArgumentNotNull(func);
        return [DebuggerStepThrough] () => func(create(), arg);
    }

    /// <summary>
    /// Combines two functions and an action into a single function.
    /// </summary>
    /// <typeparam name="TResult1">The type of the first result.</typeparam>
    /// <typeparam name="TResult2">The type of the second result.</typeparam>
    /// <typeparam name="TArg">The type of the argument.</typeparam>
    /// <param name="create">The first function.</param>
    /// <param name="func">The second function.</param>
    /// <param name="action">The action.</param>
    /// <param name="arg">The argument.</param>
    /// <returns>A function that combines the two functions and the action.</returns>
    public static Func<TResult2> Compose<TResult1, TResult2, TArg>([DisallowNull] this Func<TResult1> create, Func<TResult1, TResult2> func, Action<TArg> action, TArg arg)
    {
        Check.MustBeArgumentNotNull(create);
        Check.MustBeArgumentNotNull(func);
        return [DebuggerStepThrough] () =>
        {
            action?.Invoke(arg);
            return func(create());
        };
    }

    /// <summary>
    /// Creates a new function that composes a given function with an action that takes an argument.
    /// </summary>
    /// <typeparam name="TResult1">The type of the result of the created function.</typeparam>
    /// <typeparam name="TArg">The type of the argument of the action.</typeparam>
    /// <param name="create">The function to compose.</param>
    /// <param name="action">The action to compose.</param>
    /// <param name="arg">The argument of the action.</param>
    /// <returns>A new function that composes the given function with the action.</returns>
    public static Func<TResult1> Compose<TResult1, TArg>([DisallowNull] this Func<TResult1> create, Action<TResult1, TArg> action, TArg arg)
    {
        Check.MustBeArgumentNotNull(create);
        return [DebuggerStepThrough] () =>
        {
            var result = create();
            action?.Invoke(result, arg);
            return result;
        };
    }

    /// <summary>
    /// Combines two functions into one, where the first function is executed and then the second one.
    /// </summary>
    /// <typeparam name="TResult1">The type of the result of the first function.</typeparam>
    /// <typeparam name="TArg">The type of the argument of the second function.</typeparam>
    /// <param name="create">The first function.</param>
    /// <param name="action">The second function.</param>
    /// <param name="arg">The argument of the second function.</param>
    /// <returns>A combined function.</returns>
    public static Func<TResult1> Compose<TResult1, TArg>([DisallowNull] this Func<TResult1> create, Action<TArg> action, TArg arg)
    {
        Check.MustBeArgumentNotNull(create);
        return [DebuggerStepThrough] () =>
        {
            var result = create();
            action?.Invoke(arg);
            return result;
        };
    }

    /// <summary>
    /// Compose a Func with an Action and a Func to get the argument for the Action.
    /// </summary>
    /// <typeparam name="TArgs">The type of the result of the Func.</typeparam>
    /// <typeparam name="TResult">The type of the argument for the Action.</typeparam>
    /// <param name="create">The Func to compose.</param>
    /// <param name="action">The Action to compose.</param>
    /// <param name="getResult">The Func to get the argument for the Action.</param>
    /// <returns>A composed Func.</returns>
    public static Func<TArgs> Compose<TArgs, TResult>([DisallowNull] this Func<TArgs> create, Action<TResult> action, Func<TArgs, TResult> getResult)
    {
        Check.MustBeArgumentNotNull(create);
        return [DebuggerStepThrough] () =>
        {
            var result = create();
            action?.Invoke(getResult(result));
            return result;
        };
    }

    /// <summary>
    /// Compose a function with an action and a getArg function.
    /// </summary>
    /// <typeparam name="TResult1">The type of the result of the create function.</typeparam>
    /// <typeparam name="TArg">The type of the argument of the action.</typeparam>
    /// <param name="create">The function to be composed.</param>
    /// <param name="action">The action to be composed.</param>
    /// <param name="getArg">The getArg function to be composed.</param>
    /// <returns>The composed function.</returns>
    public static Func<TResult1> Compose<TResult1, TArg>([DisallowNull] this Func<TResult1> create, Action<TResult1, TArg> action, Func<TResult1, TArg> getArg)
    {
        Check.MustBeArgumentNotNull(create);
        return [DebuggerStepThrough] () =>
        {
            var result = create();
            action?.Invoke(result, getArg(result));
            return result;
        };
    }

    /// <summary>
    /// Creates a new function that composes two functions together.
    /// </summary>
    /// <typeparam name="TResult1">The type of the first result.</typeparam>
    /// <typeparam name="TResult2">The type of the second result.</typeparam>
    /// <typeparam name="TArg">The type of the argument.</typeparam>
    /// <param name="create">The first function.</param>
    /// <param name="func">The second function.</param>
    /// <param name="getArg">The argument for the second function.</param>
    /// <returns>A new function that composes the two functions.</returns>
    public static Func<TResult2> Compose<TResult1, TResult2, TArg>([DisallowNull] this Func<TResult1> create, Func<TResult1, TArg, TResult2> func, Func<TResult1, TArg> getArg)
    {
        Check.MustBeArgumentNotNull(create);
        Check.MustBeArgumentNotNull(func);
        return [DebuggerStepThrough] () =>
        {
            var start = create();
            return func(start, getArg(start));
        };
    }

    /// <summary> Compose a Func<TResult1> with a Func<TResult1, TResult2> and an Action<TArg> and a
    /// Func<TResult1, TArg> </summary> <typeparam name="TResult1">The type of the result of the
    /// first Func</typeparam> <typeparam name="TResult2">The type of the result of the second
    /// Func</typeparam> <typeparam name="TArg">The type of the argument of the Action</typeparam>
    /// <param name="create">The first Func</param> <param name="func">The second Func</param>
    /// <param name="action">The Action</param> <param name="getArg">The Func to get the argument of
    /// the Action</param> <returns>A Func<TResult2></returns> <exception
    /// cref="ArgumentNullException">Thrown when create is null</exception>
    public static Func<TResult2> Compose<TResult1, TResult2, TArg>([DisallowNull] this Func<TResult1> create, Func<TResult1, TResult2> func, Action<TArg> action, Func<TResult1, TArg> getArg)
    {
        Check.MustBeArgumentNotNull(create);
        Check.MustBeArgumentNotNull(func);
        return [DebuggerStepThrough] () =>
        {
            var start = create();
            action?.Invoke(getArg(start));
            return func(start);
        };
    }

    /// <summary>
    /// Disposes the specified disposable object.
    /// </summary>
    /// <typeparam name="TDisposable">The type of the disposable.</typeparam>
    /// <param name="disposable">The disposable.</param>
    /// <param name="action">The action.</param>
    public static void Dispose<TDisposable>(in TDisposable disposable, in Action<TDisposable>? action = null) where TDisposable : IDisposable
        => Dispose(disposable, action);

    /// <summary>
    /// Disposes the specified disposable object.
    /// </summary>
    /// <typeparam name="TDisposable">The type of the disposable.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="disposable">The disposable.</param>
    /// <param name="action">The action.</param>
    /// <returns></returns>
    public static TResult Dispose<TDisposable, TResult>(in TDisposable disposable, in Func<TDisposable, TResult> action) where TDisposable : IDisposable
        => Dispose(disposable, action);

    /// <summary>
    /// Disposes the specified disposable object.
    /// </summary>
    /// <typeparam name="TDisposable">The type of the disposable.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="disposable">The disposable.</param>
    /// <param name="result">The result.</param>
    /// <returns></returns>
    public static TResult Dispose<TDisposable, TResult>(in TDisposable disposable, in TResult result) where TDisposable : IDisposable
        => Dispose(disposable, result);

    /// <summary>
    /// Disposes the specified disposable object.
    /// </summary>
    /// <typeparam name="TDisposable">The type of the disposable.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="disposable">The disposable.</param>
    /// <param name="action">The action.</param>
    /// <returns></returns>
    public static TResult Dispose<TDisposable, TResult>(in TDisposable disposable, in Func<TResult> action) where TDisposable : IDisposable
        => Dispose(disposable, action);

    /// <summary>
    /// Disposes the specified disposable object.
    /// </summary>
    /// <typeparam name="TDisposable">The type of the disposable.</typeparam>
    /// <param name="disposable">The disposable.</param>
    /// <param name="action">The action.</param>
    public static void Dispose<TDisposable>(this TDisposable disposable, in Action<TDisposable>? action = null)
        where TDisposable : IDisposable
    {
        try
        {
            action?.Invoke(disposable);
        }
        finally
        {
            disposable?.Dispose();
        }
    }

    /// <summary>
    /// Disposes the specified disposable object.
    /// </summary>
    /// <typeparam name="TDisposable">The type of the disposable.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="disposable">The disposable.</param>
    /// <param name="action">The action.</param>
    /// <returns></returns>
    public static TResult Dispose<TDisposable, TResult>(this TDisposable disposable, in Func<TDisposable, TResult> action)
        where TDisposable : IDisposable
    {
        Check.MustBeArgumentNotNull(action);
        try
        {
            return action(disposable);
        }
        finally
        {
            disposable?.Dispose();
        }
    }

    /// <summary>
    /// Disposes the specified disposable object.
    /// </summary>
    /// <typeparam name="TDisposable">The type of the disposable.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="disposable">The disposable.</param>
    /// <param name="result">The result.</param>
    /// <returns></returns>
    public static TResult Dispose<TDisposable, TResult>(this TDisposable disposable, in TResult result)
        where TDisposable : IDisposable
    {
        try
        {
            return result;
        }
        finally
        {
            disposable?.Dispose();
        }
    }

    /// <summary>
    /// Disposes the specified disposable object.
    /// </summary>
    /// <typeparam name="TDisposable">The type of the disposable.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="disposable">The disposable.</param>
    /// <param name="action">The action.</param>
    /// <returns></returns>
    public static TResult Dispose<TDisposable, TResult>(this TDisposable disposable, in Func<TResult> action)
        where TDisposable : IDisposable
    {
        Check.MustBeArgumentNotNull(action);
        try
        {
            return action();
        }
        finally
        {
            disposable?.Dispose();
        }
    }

    public static void Finally(Action action, Action done)
    {
        try
        {
            action?.Invoke();
        }
        finally
        {
            done?.Invoke();
        }
    }

    public static TInstance For<TInstance, TItem>(in TInstance @this, IEnumerable<TItem>? items, in Action<(TItem Item, int Index)> action, Func<int, int>? getNext = null)
    {
        var act = action;
        For(items, (TItem item, int index) => act((item, index)), getNext);
        return @this;
    }

    public static TInstance For<TInstance, TItem>(in TInstance @this, IEnumerable<TItem>? items, in Action<TItem, int> action, Func<int, int>? getNext = null)
    {
        var act = action;
        For(items, (TItem item, int index) => act(item, index), getNext);
        return @this;
    }

    /// <summary>
    /// Loop through the items and execute the action for each item.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="items"></param>
    /// <param name="action"></param>
    /// <param name="getNext"></param>
    public static void For<TItem>(IEnumerable<TItem>? items, in Action<(TItem Item, int Index)> action, Func<int, int>? getNext = null)
    {
        var act = action;
        For(items, (TItem item, int index) => act((item, index)), getNext);
    }

    /// <summary>
    /// Loop through the items and execute the action for each item. This method is a replacement for the keyword <code>for</code>.
    /// <code>for</code>
    /// keyword.
    /// </summary>
    /// <typeparam name="TItem">The type of the items in the IEnumerable.</typeparam>
    /// <param name="items">The IEnumerable of items.</param>
    /// <param name="action">The action to execute for each item.</param>
    /// <param name="getNext">A function to get the next index.</param>
    public static void For<TItem>(IEnumerable<TItem>? items, in Action<TItem, int> action, Func<int, int>? getNext = null)
    {
        //If the action is null, return
        if (action is null || items is not null)
        {
            return;
        }

        //Set the getNext variable to increment the index
        getNext ??= i => ++i;

        //If the items are an array, loop through the array and execute the action
        if (items is TItem[] arr)
        {
            for (var i = 0; i < arr.Length; i = getNext(i))
            {
                action(arr[i], i);
            }
        }
        //If the items are a list, loop through the list and execute the action
        else if (items is IList<TItem> list)
        {
            for (var i = 0; i < list.Count; i = getNext(i))
            {
                action(list[i], i);
            }
        }
        //If the items are not null, loop through the items and execute the action
        else if (items is not null)
        {
            var enumerator = items.GetEnumerator();
            var index = 0;
            while (enumerator.MoveNext())
            {
                action(enumerator.Current, index);
                index = getNext(index);
            }
        }
    }

    /// <summary>
    /// Loop through the items and execute the action for each item.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="max"></param>
    /// <param name="selector"></param>
    /// <returns></returns>
    public static IEnumerable<T> For<T>(int max, Func<int, T> selector)
    {
        Check.MustBeArgumentNotNull(selector);

        for (var i = 0; i < max; i++)
        {
            yield return selector(i);
        }
    }

    /// <summary>
    /// Loops through the items and executes the action for each item.
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <typeparam name="Item"></typeparam>
    /// <param name="this"></param>
    /// <param name="items"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static TInstance ForEach<TInstance, Item>(in TInstance @this, IEnumerable<Item>? items, in Action<Item> action)
    {
        if (items is null || action is null)
        {
            return @this;
        }

        foreach (var item in items)
        {
            action(item);
        }

        return @this;
    }

    /// <summary>
    /// Gets the caller method.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <returns></returns>
    public static MethodBase? GetCallerMethod(in int index = 2)
        => new StackTrace(true).GetFrame(index)?.GetMethod();

    /// <summary>
    /// Gets the caller method from the stack trace.
    /// </summary>
    /// <param name="index">The index of the stack frame.</param>
    /// <param name="parsePrevIfNull">
    /// Whether to parse the previous frame if the current one is null.
    /// </param>
    /// <returns>The method base of the caller.</returns>
    [Obsolete("Subject to remove", true)]
    public static MethodBase? GetCallerMethod(in int index, bool parsePrevIfNull)
    {
        // Create a new StackTrace object
        var stackTrace = new StackTrace(true);
        // Set the index to the parameter passed in
        var i = index;
        // If the frame at the index is not null, or parsePrevIfNull is false, return the method at
        // the index
        if (stackTrace.GetFrame(i) is not null || parsePrevIfNull is false)
        {
            return stackTrace.GetFrame(i)?.GetMethod();
        }

        // If the frame at the index is null, decrement the index until a non-null frame is found
        while (stackTrace.GetFrame(--i) is null)
        {
            if (i == 0)
            {
                break;
            }
        }

        // Return the method at the non-null frame
        return stackTrace.GetFrame(i)?.GetMethod();
    }

    /// <summary>
    /// Gets the name of the caller method.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <returns></returns>
    public static string? GetCallerMethodName(in int index = 2)
        => GetCallerMethod(index)?.Name;

    /// <summary>
    /// If b returns true then invoke the ifTrue function else invoke the ifFalse function.
    /// </summary>
    /// <param name="b"></param>
    /// <param name="ifTrue"></param>
    /// <param name="ifFalse"></param>
    public static void If(Func<bool> b, in Action ifTrue, in Action? ifFalse = null)
    {
        if (b())
        {
            ifTrue?.Invoke();
        }
        else
        {
            ifFalse?.Invoke();
        }
    }

    /// <summary>
    /// If b returns true then invoke the ifTrue function else invoke the ifFalse function.
    /// </summary>
    /// <param name="b"></param>
    /// <param name="ifTrue"></param>
    /// <param name="ifFalse"></param>
    public static void If(bool b, in Action ifTrue, in Action? ifFalse = null)
    {
        if (b)
        {
            ifTrue?.Invoke();
        }
        else
        {
            ifFalse?.Invoke();
        }
    }

    /// <summary>
    /// If b is true then invoke the ifTrue function else invoke the ifFalse function.
    /// </summary>
    /// <param name="b"></param>
    /// <param name="ifTrue"></param>
    /// <param name="ifFalse"></param>
    /// <returns></returns>
    public static Task If(bool b, in Func<Task> ifTrue, in Func<Task>? ifFalse = null) =>
        b
            ? ifTrue()
            : ifFalse?.Invoke()
                ?? Task.CompletedTask;

    /// <summary>
    /// If b is true then invoke the ifTrue function else invoke the ifFalse function.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="ifTrue"></param>
    /// <param name="ifFalse"></param>
    /// <returns></returns>
    public static Task IfNotNull(object? obj, in Func<Task> ifTrue, in Func<Task>? ifFalse = null) =>
        obj is not null
            ? ifTrue()
            : ifFalse?.Invoke()
                ?? Task.CompletedTask;

    /// <summary>
    /// If b is true then invoke the ifTrue function else invoke the ifFalse function.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="ifTrue"></param>
    /// <param name="ifFalse"></param>
    /// <returns></returns>
    public static Task<Result> IfNotNull(object? obj, in Func<Task<Result>> ifTrue, in Func<Task<Result>>? ifFalse = null) =>
        obj is not null
            ? ifTrue()
            : ifFalse?.Invoke()
                ?? Task.FromResult(Result.Success());

    /// <summary>
    /// If b is false then invoke the ifFalse function.
    /// </summary>
    /// <param name="b"></param>
    /// <param name="ifFalse"></param>
    /// <returns></returns>
    public static bool IfFalse(this bool b, in Action ifFalse)
    {
        if (b is false)
        {
            ifFalse?.Invoke();
        }
        return b;
    }

    /// <summary>
    /// If b is false then invoke the ifFalse function else return the default value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="b"></param>
    /// <param name="ifFalse"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static T IfFalse<T>(this bool b, in Func<T> ifFalse, in T defaultValue = default!)
        => b is false ? ifFalse.Invoke() : defaultValue;

    /// <summary>
    /// If b is true then invoke the ifTrue function.
    /// </summary>
    /// <param name="b"></param>
    /// <param name="ifTrue"></param>
    /// <returns></returns>
    public static bool IfTrue(this bool b, in Action ifTrue)
    {
        if (b is true)
        {
            ifTrue?.Invoke();
        }
        return b;
    }

    /// <summary>
    /// If b is true then invoke the ifTrue function else return the default value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="b"></param>
    /// <param name="ifTrue"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static T IfTrue<T>(this bool b, in Func<T> ifTrue, in T defaultValue = default!)
        => b is true ? ifTrue.Invoke() : defaultValue;

    /// <summary>
    /// Locks the specified lock object.
    /// </summary>
    /// <param name="lockObject"></param>
    /// <param name="action"></param>
    public static void Lock(object? lockObject, Action action)
        => _ = Lock(lockObject, ()
            =>
            {
                action.ArgumentNotNull(nameof(action))();
                return true;
            });

    /// <summary>
    /// Locks the specified lock object.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="lockObject"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static TResult Lock<TResult>(object? lockObject, in Func<TResult> action)
    {
        lock (lockObject ?? CodeHelper.GetCallerMethod()!.DeclaringType!)
        {
            return action.ArgumentNotNull()();
        }
    }

    /// <summary>
    /// Creates a new instance of the generic type T.
    /// </summary>
    /// <typeparam name="T">The type of the instance to create.</typeparam>
    /// <returns>A new instance of the generic type T.</returns>
    public static T New<T>()
            where T : class, new()
            => new();

    /// <summary>
    /// Creates a new instance of the generic type T.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="args"></param>
    /// <returns></returns>
    public static T New<T>(params object[] args)
        => Activator.CreateInstance(typeof(T), args).NotNull().Cast().To<T>();

    /// <summary>
    /// Throws a new instance of the specified exception type.
    /// </summary>
    [DoesNotReturn]
    public static void Throw<TException>() where TException : Exception, new()
        => throw new TException();

    /// <summary>
    /// Throws the specified exception.
    /// </summary>
    /// <typeparam name="TFakeResult">The type of the return value.</typeparam>
    /// <param name="exception">The exception to throw.</param>
    /// <returns>This method does not return.</returns>
    [DoesNotReturn]
    public static TFakeResult Throw<TFakeResult>(in Exception exception) =>
        throw exception;

    /// <summary>
    /// Throws the specified exception.
    /// </summary>
    /// <param name="exception">The exception to throw.</param>
    [DoesNotReturn]
    public static void Throw(in Exception exception) =>
        throw exception;

    /// <summary>
    /// Throws an exception using the provided function.
    /// </summary>
    [DoesNotReturn]
    public static T Throw<T>(in Func<Exception> getException) =>
        throw getException();

    /// <summary>
    /// Throws an exception using the provided Func.
    /// </summary>
    [DoesNotReturn]
    public static void Throw(in Func<Exception> getException) =>
        throw getException();

    /// <summary>
    /// Executes the specified action and throws an exception if an error occurs.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="action">The action to execute.</param>
    /// <param name="getException">A function to get the exception to throw.</param>
    /// <returns>The result of the action.</returns>
    public static TResult ThrowOnError<TResult>(in Func<TResult> action, Func<Exception, Exception>? getException = null)
    {
        Check.MustBeArgumentNotNull(action);
        try
        {
            return action();
        }
        catch (Exception ex)
        {
            if (getException is not null)
            {
                Throw(() => getException(ex));
            }

            throw;
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

    public static IEnumerable<TResult> While<TResult>(Func<bool> predicate, Func<TResult> action, Action? onIterationDone = null)
    {
        Check.MustBeArgumentNotNull(predicate);
        Check.MustBeArgumentNotNull(action);

        while (predicate())
        {
            yield return action();
        }

        onIterationDone?.Invoke();
    }

    /// <summary>
    /// Executes an action while a predicate is true.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="action"></param>
    public static void While(in Func<bool> predicate, in Action? action = null)
    {
        Check.MustBeArgumentNotNull(predicate);

        while (predicate())
        {
            action?.Invoke();
        }
    }

    /// <summary>
    /// Invokes an action on an instance and returns the instance.
    /// </summary>
    /// <param name="instance">The instance to invoke the action on.</param>
    /// <param name="action">The action to invoke.</param>
    /// <returns>The instance.</returns>
    public static TInstance With<TInstance>(this TInstance instance, in Action<TInstance>? action)
    {
        action?.Invoke(instance);
        return instance;
    }

    /// <summary>
    /// Executes an action on a task instance and returns the result.
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <param name="instance"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static TInstance With<TInstance>(this TInstance instance, in Func<TInstance, TInstance> action)
    {
        return action(instance);
    }

    /// <summary>
    /// Executes an action on a task instance and returns the result.
    /// </summary>
    /// <param name="instanceAsync">The task instance.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The result of the task instance.</returns>
    public static async Task<TInstance> WithAsync<TInstance>(this Task<TInstance> instanceAsync, Action<TInstance>? action)
    {
        var result = await instanceAsync;
        action?.Invoke(result);
        return result;
    }
}