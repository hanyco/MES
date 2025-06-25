using System.Collections;
using System.Collections.Immutable;

namespace Library.Extensions;

public static class EnumerableExtension
{
    extension(IEnumerable @this)
    {
        // 'source' refers to receiver
        public bool IsEmpty => @this switch
        {
            null => true,
            Array a => a.Length == 0,
            ICollection list => list.Count == 0,
            _ => @this.GetEnumerator().MoveNext()
        };
    }

    extension<TItem>(IEnumerable<TItem> @this)
    {
        public bool IsEmpty => @this switch
        {
            null => true,
            Array a => a.Length == 0,
            ICollection list => list.Count == 0,
            _ => !@this.Any()
        };

        public void ForEach(Action<TItem> action)
        {
            var items = @this.ToImmutableArray();
            foreach (var item in items)
            {
                action(item);
            }
        }

        public IEnumerable<TItem> AddRangeImmuted(params TItem[] items)
        {
            foreach (var item in @this)
            {
                yield return item;
            }
            foreach (var item in items)
            {
                yield return item;
            }
        }

        public IEnumerable<TItem> Compact() =>
            @this?.Where(x => x is not null) ?? [];

        public IEnumerable<TItem> AddImmuted(TItem item)
        {
            foreach (var i in @this)
            {
                yield return i;
            }
            yield return item;
        }

        /// <summary>
        /// Returns a collection of elements from the input sequence that do not satisfy the
        /// specified predicate.
        /// </summary>
        public IEnumerable<TItem> Except(Func<TItem, bool> exceptor)
        {
            Check.MustBeArgumentNotNull(exceptor);

            return @this.Where(x => !exceptor(x));
        }

        public TResult? SelectImmutable<TResult>(in Func<TItem?, TResult?, TResult?> selector, in TResult? defaultResult = default)
        {
            var result = defaultResult;
            if (@this is { } && @this.Any())
            {
                Check.MustBeArgumentNotNull(selector);
                foreach (var item in @this)
                {
                    result = selector(item, result);
                }
            }
            return result;
        }
    }

    extension<TKey, TValue>(IEnumerable<(TKey Key, TValue Value)> @this)
    {
        /// <summary>
        /// Checks if the given IEnumerable contains a key-value pair with the specified key.
        /// </summary>
        public bool ContainsKey(TKey key) =>
            @this.ArgumentNotNull().Any(kv => kv.Key?.Equals(key) ?? key is null);

        /// <summary>
        /// Gets the value from the given source by the specified key.
        /// </summary>
        /// <typeparam name="TKey"> The type of the key. </typeparam>
        /// <typeparam name="TValue"> The type of the value. </typeparam>
        /// <param name="source"> The source. </param>
        /// <param name="key">    The key. </param>
        /// <returns> The value. </returns>
        public TValue GetValueByKey(TKey key) =>
            @this.ArgumentNotNull().First(kv => kv.Key?.Equals(key) ?? key is null).Value;
    }

    extension(Enumerable)
    {
        public static IEnumerable<T> ToEnumerable<T>(T item)
        {
            yield return item;
        }

        public static IEnumerable<TItem> Iterate<TItem>(IEnumerable<TItem> items)
        {
            using var enumerator = items.GetEnumerator();
            while (enumerator.MoveNext())
            {
                yield return enumerator.Current;
            }
        }
    }

    /// <summary>
    /// Adds a range of items to the specified collection.
    /// </summary>
    /// <typeparam name="TList"> The type of the collection to which items will be added. </typeparam>
    /// <typeparam name="TItem"> The type of the items to be added. </typeparam>
    /// <param name="this">  The collection to which the items will be added. </param>
    /// <param name="items"> The items to be added to the collection. </param>
    /// <returns> The updated collection with added items. </returns>
    /// <remarks>
    /// This extension method allows adding a range of items to a collection that implements
    /// ICollection. The method checks if the 'items' enumerable is not null and contains items
    /// before performing the addition.
    /// </remarks>
    public static TList AddRange<TList, TItem>([DisallowNull] this TList @this, in IEnumerable<TItem> items)
        where TList : ICollection<TItem>
    {
        if (items?.Any() is true)
        {
            // Iterate through each item in the 'items' enumerable and add it to the collection.
            foreach (var item in items)
            {
                @this.Add(item);
            }
        }
        return @this; // Return the updated collection with added items.
    }

    /// <summary>
    /// Selects all elements from a sequence of sequences.
    /// </summary>
    /// <param name="this"> The sequence of sequences. </param>
    /// <returns> A sequence containing all elements of the input sequences. </returns>
    public static IEnumerable<TItem> SelectAll<TItem>(this IEnumerable<IEnumerable<TItem>> @this)
    {
        Check.MustBeArgumentNotNull(@this);
        foreach (var value in @this)
        {
            foreach (var item in value)
            {
                yield return item;
            }
        }
    }

    public static async Task<List<TItem>> ToListAsync<TItem>(this Task<IEnumerable<TItem>> itemsTask, CancellationToken cancellationToken = default)
    {
        var items = await itemsTask.ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();
        var result = items.ToList();
        return result;
    }
}