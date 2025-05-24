using System.Collections;
using System.Collections.Immutable;

using Library.Extensions;
using Library.Validations;

namespace Library.Extensions;

public static class EnumerableExtension
{
    extension(IEnumerable source)
    {
        // 'source' refers to receiver
        public bool IsEmpty => !source.GetEnumerator().MoveNext();
    }

    extension<TItem>(IEnumerable<TItem> source)
    {
        // 'source' refers to receiver
        public bool IsEmpty => !source.GetEnumerator().MoveNext();

        public void ForEach(Action<TItem> action)
        {
            var items = source.ToImmutableArray();
            foreach (var item in items)
            {
                action(item);
            }
        }

        public IEnumerable<TItem> AddRangeImmuted(params TItem[] items)
        {
            foreach (var item in source)
            {
                yield return item;
            }
            foreach (var item in items)
            {
                yield return item;
            }
        }

        public IEnumerable<TItem> Compact()
        {
            foreach (var item in source)
            {
                if (item is not null)
                {
                    yield return item;
                }
            }
        }

        public IEnumerable<TItem> AddImmuted(TItem item)
        {
            foreach (var i in source)
            {
                yield return i;
            }
            yield return item;
        }
    }

    /// <summary>
    /// Selects all elements from a sequence of sequences.
    /// </summary>
    /// <param name="values">The sequence of sequences.</param>
    /// <returns>A sequence containing all elements of the input sequences.</returns>
    public static IEnumerable<T> SelectAll<T>(this IEnumerable<IEnumerable<T>> values)
    {
        Check.MustBeArgumentNotNull(values);

        foreach (var value in values)
        {
            foreach (var item in value)
            {
                yield return item;
            }
        }
    }

    public static IEnumerable<T> AsEnumerable<T>(T item)
    {
        yield return item;
    }
}