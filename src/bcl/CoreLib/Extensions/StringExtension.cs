using System.Diagnostics.CodeAnalysis;

using Library.Extensions;

using NullableStrings = System.Collections.Generic.IEnumerable<string?>;
using SpanString = System.ReadOnlySpan<char>;
using Strings = System.Collections.Generic.IEnumerable<string>;


namespace Library.Extensions;

public static class StringExtension
{
    extension(string? str)
    {//ولی من خیلی دوستدارم حتی با خشم
        public int Length => str?.Length ?? 0;

        public string AsNotNull() =>
            str is null ? string.Empty : str;

        public string? Trim() => str?.Trim();

        [return: NotNullIfNotNull(nameof(str))]
        public string? RemoveEnd([DisallowNull] string? suffix) =>
            str?.EndsWith(suffix) == true ? str[..^suffix.Length] : str;

        /// <summary>
        /// Removes the specified value from the string.
        /// </summary>
        /// <param name="str">The string to remove the value from.</param>
        /// <param name="value">The value to remove.</param>
        /// <returns>The string with the value removed, or null if the string is null.</returns>
        [return: NotNullIfNotNull(nameof(value))]
        public string? Remove(in string? value) =>
            value is null ? str : str?.Replace(value, "");

        [return: NotNullIfNotNull(nameof(str))]
        public string? IfNullOrEmpty(in string? substitute) =>
            string.IsNullOrEmpty(str) ? substitute : str;

        /// <summary>
        /// Reads a large string line by line.
        /// </summary>
        /// <param name="str">The large string to read.</param>
        /// <returns>An enumerable of strings representing each line of the input string.</returns>
        public Strings ReadLines()
        {
            if (str.IsNullOrEmpty())
            {
                yield break;
            }

            using var reader = new StringReader(str);
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                yield return line;
            }
        }
    }
    extension(string str)
    {
        /// <summary>
        /// Replaces all occurrences of a character in a string with a new character.
        /// </summary>
        public string ReplaceAll(in IEnumerable<(char OldValue, char NewValue)> items) =>
            items.Aggregate(str, (current, item) => current.Replace(item.OldValue, item.NewValue));

        /// <summary>
        /// Replaces all occurrences of the specified old values with the specified new value in the
        /// given string.
        /// </summary>
        public string ReplaceAll(Strings oldValues, string newValue) =>
            oldValues.Aggregate(str, (current, oldValue) => current.Replace(oldValue, newValue));
    }
    extension([NotNullWhen(false)][AllowNull] string? str)
    {
        public bool IsNullOrEmpty() =>
            string.IsNullOrEmpty(str);

        public bool IsNullOrWhiteSpace() =>
            string.IsNullOrWhiteSpace(str);

        public bool IsEqualTo(string? other, bool ignoreCase = true) =>
            string.Equals(str, other, StringComparison.InvariantCultureIgnoreCase);
    }
    extension(NullableStrings text)
    {
        public string Merge(string separator = ", ") =>
            string.Join(separator, text.Where(t => t is not null));

        public string Merge(char separator = ',') =>
            string.Join(separator, text.Where(t => t is not null));
    }
}