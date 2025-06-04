using System.Globalization;

namespace Library.Globalization;

public sealed class PersianCultureInfo : CultureInfo
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="PersianCultureInfo" /> class.
    /// </summary>
    /// <param name="cultureName">Name of the culture.</param>
    /// <param name="useUserOverride">if set to <c>true</c> [use user override].</param>
    public PersianCultureInfo(string cultureName = "fa-IR", bool useUserOverride = true)
        : base(cultureName, useUserOverride)
    {
        //Temporary Value for _Calendar.
        this.Calendar = base.OptionalCalendars[0];

        //populating new list of optional calendars.
        var optionalCalendars = new List<Calendar>();
        optionalCalendars.AddRange(base.OptionalCalendars);
        var persianCalendar = new PersianCalendar();
        optionalCalendars.Insert(0, persianCalendar);

        this.OptionalCalendars = optionalCalendars.ToArray();
        this.Calendar = this.OptionalCalendars[0];
        var dateTimeFormatInfo = new DateTimeFormatInfo
        {
            AbbreviatedMonthGenitiveNames = PersianDateTime.PersianMonthNameAbbrsInPersian.AddImmuted(" ").ToArray(),
            AbbreviatedMonthNames = PersianDateTime.EnglishMonthNameAbbrsInPersian.AddImmuted(" ").ToArray(),
            AMDesignator = CultureConstants.AM_DESIGNATOR,
            CalendarWeekRule = CalendarWeekRule.FirstFourDayWeek,
            DateSeparator = CultureConstants.DATE_SEPARATOR,
            DayNames = PersianDateTime.PersianWeekDays.Select(wd => wd.Name).ToArray(),
            FirstDayOfWeek = DayOfWeek.Saturday,
            FullDateTimePattern = CultureConstants.FULL_DATE_TIME_PATTERN,
            LongDatePattern = CultureConstants.LONG_DATE_PATTERN,
            AbbreviatedDayNames = PersianDateTime.PersianWeekDaysAbbrs.Select(abbr => abbr.Name).ToArray(),
            //dateTimeFormatInfo.Calendar = persianCalendar;
            LongTimePattern = CultureConstants.LONG_TIME_PATTERN,
            MonthGenitiveNames = PersianDateTime.PersianMonthNamesInGenitive.AddImmuted(" ").ToArray(),
            MonthNames = PersianDateTime.PersianMonthNamesInPersian.AddImmuted(" ").ToArray(),
            PMDesignator = CultureConstants.PM_DESIGNATOR,
            ShortDatePattern = CultureConstants.SHORT_DATE_PATTERN,
            ShortTimePattern = CultureConstants.SHORT_TIME_PATTERN,
            ShortestDayNames = PersianDateTime.PersianWeekDaysAbbrs.Select(wd => wd.Name).ToArray(),
            TimeSeparator = CultureConstants.TIME_SEPARATOR
        };
        this.DateTimeFormat = dateTimeFormatInfo;
    }

    /// <summary>
    ///     Gets the default calendar used by the culture.
    /// </summary>
    public override Calendar Calendar { get; }

    /// <summary>
    ///     Gets the list of calendars that can be used by the culture.
    /// </summary>
    public override Calendar[] OptionalCalendars { get; }
}
