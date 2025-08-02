using System.Globalization;

using Library.DesignPatterns.Creational;

namespace Library.Globalization.Helpers;

public static class Localization
{
    /// <summary>
    /// Converts a string to a cultural number based on the specified language and corrects Persian
    /// characters if needed.
    /// </summary>
    /// <param name="value">               The string to convert. </param>
    /// <param name="correctPersianChars">
    /// A boolean value indicating whether Persian characters should be corrected.
    /// </param>
    /// <param name="toLanguage">          The language to convert the string to. </param>
    /// <returns> The converted string. </returns>
    [return: NotNullIfNotNull(nameof(value))]
    public static string ToCulturalNumber(in string value, in Language toLanguage) =>
        toLanguage switch
        {
            Language.Persian => ToPersianDigits(value),
            Language.English => value.ToEnglishDigits(),
            Language.None => throw new NotImplementedException(),
            _ => value
        };

    /// <summary>
    /// Replaces all Persian digits in the given string with their English equivalents.
    /// </summary>
    [return: NotNullIfNotNull(nameof(value))]
    public static string ToEnglishDigits(this string value) =>
        value.ReplaceAll(PersianTools.Digits.Select(n => (n.Persian, n.English)));

    /// <summary>
    /// Replaces all English digits in the given string with their Persian equivalents.
    /// </summary>
    [return: NotNullIfNotNull(nameof(value))]
    public static string ToPersianDigits(in string value) =>
        value.ReplaceAll(PersianTools.Digits.Select(n => (n.English, n.Persian)));

    /// <summary>
    /// Gets or sets the localizer.
    /// </summary>
    /// <value> The localizer. </value>
    /// <exception cref="NotImplementedException"> </exception>
    public static ILocalizer Localizer
    {
        get => field ??= InvariantCultureLocalizer.Instance;
        set;
    }

    /// <summary>
    /// Converts to local string.
    /// </summary>
    /// <param name="dateTime"> The date time. </param>
    /// <returns> </returns>
    public static string ToLocalString(this PersianDateTime dateTime)
        => Localizer.ToString(dateTime);

    /// <summary>
    /// Converts to local string.
    /// </summary>
    /// <param name="dateTime"> The date time. </param>
    /// <returns> </returns>
    [return: NotNull]
    public static string ToLocalString(this DateTime dateTime)
        => Localizer?.ToString(dateTime) ?? dateTime.ToString(CultureInfo.CurrentCulture);
}

public sealed class CurrentCultureLocalizer : LocalizerBase<CurrentCultureLocalizer>
{
    private CurrentCultureLocalizer()
        : base(CultureInfo.CurrentCulture) { }
}

public sealed class EnglishLocalizer : LocalizerBase<EnglishLocalizer>
{
    private EnglishLocalizer()
        : base(CultureInfo.GetCultureInfo("en-US")) { }
}

public sealed class InvariantCultureLocalizer : LocalizerBase<InvariantCultureLocalizer>
{
    private InvariantCultureLocalizer()
        : base(CultureInfo.InvariantCulture) { }
}

public abstract class LocalizerBase<TLocalizer>(in CultureInfo culture) : Singleton<TLocalizer>, ILocalizer
    where TLocalizer : LocalizerBase<TLocalizer>
{
    protected CultureInfo CultureInfo { get; } = culture;

    public string ToString(in DateTime dateTime)
        => dateTime.ToString(CultureConstants.DEFAULT_DATE_TIME_PATTERN, this.CultureInfo);

    public string Translate(in string statement, in string? culture = null)
        => throw new NotImplementedException();
}

public sealed class PersianLocalizer : LocalizerBase<PersianLocalizer>
{
    private PersianLocalizer()
        : base(CultureInfo.GetCultureInfo("fa-IR")) { }
}