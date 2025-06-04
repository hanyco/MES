using System.Globalization;
using System.Text.RegularExpressions;
using Library.Validations;

namespace Library.Globalization.Pluralization;

public static class Pluralizer
{
    private static readonly IEnumerable<(Regex Key, string Value)> _pluralRules = Rules.PluralRules;
    private static readonly IEnumerable<(Regex Key, string Value)> _singularRules = Rules.SingularRules;
    private static readonly IEnumerable<string> _uncountables = Rules.Uncountables;
    private static readonly IEnumerable<(string Key, string Value)> _irregularPlurals = Rules.GetIrregularPlurals();
    private static readonly IEnumerable<(string Key, string Value)> _irregularSingles = Rules.GetIrregularSingulars();
    private static readonly Regex _replacementRegex = new("\\$(\\d{1,2})");

    public static string Pluralize(in string word)
        => Transform(word, _irregularSingles, _irregularPlurals, _pluralRules);

    public static string Singularize(in string word)
        => Transform(word, _irregularPlurals, _irregularSingles, _singularRules);

    internal static string RestoreCase(in string originalWord, in string newWord)
    {
        // Tokens are an exact match.
        if (originalWord == newWord)
        {
            return newWord;
        }

        // Upper cased words. E.g. "HELLO".
        if (originalWord == originalWord.ToUpper(CultureInfo.InvariantCulture))
        {
            return newWord.ToUpper(CultureInfo.InvariantCulture);
        }

        // Title cased words. E.g. "Title".
        if (originalWord[0] == char.ToUpper(originalWord[0], CultureInfo.InvariantCulture))
        {
            return char.ToUpper(newWord[0], CultureInfo.InvariantCulture) + newWord[1..];
        }

        // Lower cased words. E.g. "test".
        return newWord.ToLower(CultureInfo.InvariantCulture);
    }

    internal static string ApplyRules(in string token, in string originalWord, in IEnumerable<(Regex Key, string Value)> rules)
    {
        // Empty string or doesn't need fixing.
        if (string.IsNullOrEmpty(token) || _uncountables.Contains(token))
        {
            return RestoreCase(originalWord, token);
        }

        var length = rules.Count();

        // Iterate over the sanitization rules and use the first one to match.
        while (length-- > 0)
        {
            var (key, value) = rules.ElementAt(length);

            // If the rule passes, return the replacement.
            if (key.IsMatch(originalWord))
            {
                var match = key.Match(originalWord);
                var matchString = match.Groups[0].Value;
                return string.IsNullOrWhiteSpace(matchString)
                    ? key.Replace(originalWord, GetReplaceMethod(originalWord[match.Index - 1].ToString(), value), 1)
                    : key.Replace(originalWord, GetReplaceMethod(matchString, value), 1);
            }
        }

        return originalWord;
    }

    private static MatchEvaluator GetReplaceMethod(string originalWord, string replacement)
        => match => RestoreCase(originalWord, _replacementRegex.Replace(replacement, m => match.Groups[Convert.ToInt32(m.Groups[1].Value, CultureInfo.InvariantCulture)].Value));

    private static string Transform(
        in string word,
        in IEnumerable<(string Key, string Value)> replacables,
        in IEnumerable<(string Key, string Value)> keepables,
        in IEnumerable<(Regex Key, string Value)> rules)
    {
        var token = word.ArgumentNotNull(nameof(word)).ToLower(CultureInfo.InvariantCulture);
        return keepables.ContainsKey(token)
            ? RestoreCase(word, token)
            : replacables.ContainsKey(token) ? RestoreCase(word, replacables.GetValueByKey(token)) : ApplyRules(token, word, rules);
    }
}
