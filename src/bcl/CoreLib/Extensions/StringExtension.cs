using System.Diagnostics.Contracts;

using NullableStrings = System.Collections.Generic.IEnumerable<string?>;
using Strings = System.Collections.Generic.IEnumerable<string>;

namespace Library.Extensions;

public static class StringExtension
{
    private static readonly char[] _standardSeparators = ['\0', '\n', '\r', '\t', '_', '-'];

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
        /// <param name="str">   The string to remove the value from. </param>
        /// <param name="value"> The value to remove. </param>
        /// <returns> The string with the value removed, or null if the string is null. </returns>
        [return: NotNullIfNotNull(nameof(value))]
        public string? Remove(in string? value) =>
            value is null ? str : str?.Replace(value, "");

        [return: NotNullIfNotNull(nameof(str))]
        public string? IfNullOrEmpty(in string? substitute) =>
            string.IsNullOrEmpty(str) ? substitute : str;

        /// <summary>
        /// Reads a large string line by line.
        /// </summary>
        /// <param name="str"> The large string to read. </param>
        /// <returns> An enumerable of strings representing each line of the input string. </returns>
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

        /// <summary>
        /// Separates a string into separate words based on the provided separators.
        /// </summary>
        /// <param name="str">        The string to separate. </param>
        /// <param name="separators"> The separators to use. </param>
        /// <returns> The separated string. </returns>
        [return: NotNullIfNotNull(nameof(str))]
        public string? Separate(params char[] separators)
        {
            //If no separators are provided, use default separators
            if (separators?.Any() is not true)
            {
                separators = _standardSeparators;
            }
            //If the string is empty, return it
            if (str.IsNullOrEmpty())
            {
                return str;
            }

            //Create a StringBuilder to store the new string
            var sb = new StringBuilder();
            //Loop through each character in the string
            for (var i = 0; i < str.Length; i++)
            {
                //Determine if the character is a separator and if it should be ignored
                var (isSeparator, shouldIgnore) = determineSeparator(str[i], separators);
                //If the character is a separator and it is not the first character, add a space
                if (i > 0 && isSeparator)
                {
                    _ = sb.Append(' ');
                }
                //If the character should not be ignored, add it to the StringBuilder
                if (!shouldIgnore)
                {
                    _ = sb.Append(str[i]);
                }
            }

            //Return the new string
            return sb.ToString();

            //Function to determine if the character is a separator and if it should be ignored
            static (bool IsSeparator, bool ShouldIgnore) determineSeparator(char c, char[] separators) =>
                separators.Contains(c) ? (true, true) : (char.IsUpper(c), false);
        }
        /// <summary>
        /// Compares two strings and returns a boolean value indicating whether they are equal.
        /// </summary>
        [Pure]
        public bool EqualsTo(in string str1, bool ignoreCase = true) =>
            str?.Equals(str1, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) ?? (str1 == null);
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

    public static bool IsNullOrEmpty([NotNullWhen(false)][AllowNull] this string? str) =>
            string.IsNullOrEmpty(str);

    public static bool IsNullOrWhiteSpace([NotNullWhen(false)][AllowNull] this string? str) =>
        string.IsNullOrWhiteSpace(str);
}