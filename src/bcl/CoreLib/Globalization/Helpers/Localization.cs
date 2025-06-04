using System.Globalization;
using Library.DesignPatterns.Creational;

namespace Library.Globalization.Helpers;

public static class Localization
{
    private static ILocalizer? _localizer;

    /// <summary>
    ///     Gets or sets the localizer.
    /// </summary>
    /// <value>
    ///     The localizer.
    /// </value>
    /// <exception cref="NotImplementedException"></exception>
    public static ILocalizer Localizer
    {
        get => _localizer ??= InvariantCultureLocalizer.Instance;
        set => _localizer = value;
    }

    /// <summary>
    ///     Converts to local string.
    /// </summary>
    /// <param name="dateTime">The date time.</param>
    /// <returns></returns>
    public static string ToLocalString(this PersianDateTime dateTime)
        => Localizer.ToString(dateTime);

    /// <summary>
    ///     Converts to local string.
    /// </summary>
    /// <param name="dateTime">The date time.</param>
    /// <returns></returns>
    [return: NotNull]
    public static string ToLocalString(this DateTime dateTime)
        => Localizer?.ToString(dateTime) ?? dateTime.ToString(CultureInfo.CurrentCulture);
}

public abstract class LocalizerBase<TLocalizer> : Singleton<TLocalizer>, ILocalizer
    where TLocalizer : LocalizerBase<TLocalizer>
{
    protected LocalizerBase(in CultureInfo culture)
        => this.CultureInfo = culture;

    protected CultureInfo CultureInfo { get; }

    public string ToString(in DateTime dateTime)
        => dateTime.ToString(CultureConstants.DEFAULT_DATE_TIME_PATTERN, this.CultureInfo);

    public string Translate(in string statement, in string? culture = null)
        => throw new NotImplementedException();
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

public sealed class PersianLocalizer : LocalizerBase<PersianLocalizer>
{
    private PersianLocalizer()
        : base(CultureInfo.GetCultureInfo("fa-IR")) { }
}
