using Library.Globalization;
using Library.Globalization.DataTypes;
using Library.Validations;

using System.Globalization;

namespace Library.Helpers;

public static class DateTimeHelper
{
    /// <summary>
    /// Deconstructs a DateTime object into its individual components.
    /// </summary>
    /// <param name="dt">The DateTime object to deconstruct.</param>
    /// <param name="year">The year component of the DateTime.</param>
    /// <param name="month">The month component of the DateTime.</param>
    /// <param name="day">The day component of the DateTime.</param>
    /// <param name="hour">The hour component of the DateTime.</param>
    /// <param name="minute">The minute component of the DateTime.</param>
    /// <param name="second">The second component of the DateTime.</param>
    /// <param name="millisecond">The millisecond component of the DateTime.</param>
    public static void Deconstruct(this DateTime dt,
            out int year,
            out int month,
            out int day,
            out int hour,
            out int minute,
            out int second,
            out int millisecond) =>
        (year, month, day, hour, minute, second, millisecond) = (dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond);

    /// <summary>
    /// Deconstructs a DateTime object into its year, month, and day components.
    /// </summary>
    public static void Deconstruct(this DateTime dt, out int year, out int month, out int day) =>
        Deconstruct(dt, out year, out month, out day, out _, out _, out _, out _);

    /// <summary>
    /// Deconstructs a DateTime object into its hour, minute, second, and millisecond components.
    /// </summary>
    public static void Deconstruct(this DateTime dt, out int hour, out int minute, out int second, out int millisecond) =>
        (hour, minute, second, millisecond) = (dt.Hour, dt.Minute, dt.Second, dt.Millisecond);

    public static DateOnly GetDateOnly(this DateTime dt) =>
        DateOnly.FromDateTime(dt);

    public static TimeBand GetTimeBand(this TimeOnly source) =>
        source.Hour switch
        {
            < 6 or > 19 => TimeBand.Overnight,
            < 10 => TimeBand.MorningRush,
            < 16 => TimeBand.Daytime,
            _ => TimeBand.Eveningrush
        };

    public static TimeOnly GetTimeOnly(this DateTime dt) =>
        TimeOnly.FromDateTime(dt);

    /// <summary>
    /// Determines whether this instance start is between.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="start">The start.</param>
    /// <param name="end">The end.</param>
    /// <returns><c>true</c> if the specified start is between; otherwise, <c>false</c>.</returns>
    public static bool IsBetween(this TimeSpan source, in TimeSpan start, in TimeSpan end) =>
        source >= start && source <= end;

    /// <summary>
    /// Determines whether this instance start is between.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="start">The start.</param>
    /// <param name="end">The end.</param>
    /// <returns><c>true</c> if the specified start is between; otherwise, <c>false</c>.</returns>
    public static bool IsBetween(this DateTime source, in DateTime start, in DateTime end) =>
        source >= start && source <= end;

    /// <summary>
    /// Determines whether this instance is between.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="start">The start.</param>
    /// <param name="end">The end.</param>
    /// <returns><c>true</c> if the specified start is between; otherwise, <c>false</c>.</returns>
    public static bool IsBetween(this TimeSpan source, in string start, in string end) =>
        source.IsBetween(ToTimeSpan(start), ToTimeSpan(end));

    /// <summary>
    /// Returns true if dateTime format is valid.
    /// </summary>
    /// <param name="dateTime">The date time.</param>
    /// <returns><c>true</c> if the specified date time is valid; otherwise, <c>false</c>.</returns>
    public static bool IsValid(in string dateTime) =>
        DateTime.TryParse(dateTime, out _);

    /// <summary>
    /// Checks if the given dateTime is a weekend day according to the given culture.
    /// </summary>
    /// <param name="dateTime">The dateTime to check.</param>
    /// <param name="culture">
    /// The culture to use for the check. If not specified, the current culture is used.
    /// </param>
    /// <returns>
    /// True if the given dateTime is a weekend day according to the given culture, false otherwise.
    /// </returns>
    public static bool IsWeekend([DisallowNull] this DateTime dateTime, CultureInfo? culture = null) =>
        (culture ?? CultureInfo.CurrentCulture).GetWeekdayState(dateTime.ArgumentNotNull().DayOfWeek)
            is CultureInfoHelper.WeekdayState.Weekend or CultureInfoHelper.WeekdayState.WorkdayMorning;

    /// <summary>
    /// Converts to datetime.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns></returns>
    public static DateTime ToDateTime(this TimeSpan source) =>
        new(source.Ticks);

    public static PersianDateTime ToPersian(this DateTime dateTime) =>
        dateTime;

    /// <summary>
    /// Converts to timespan.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns></returns>
    public static TimeSpan ToTimeSpan(in string source) =>
        TimeSpan.Parse(source, CultureInfo.CurrentCulture);

    /// <summary>
    /// Converts to timespan.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns></returns>
    public static TimeSpan ToTimeSpan(this DateTime source) =>
        new(source.Ticks);
}