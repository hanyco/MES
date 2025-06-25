using System.Globalization;

using Library.Globalization;
using Library.Globalization.DataTypes;

namespace Library.Extensions;

public static class DateTimeExtension
{
    extension(DateTime)
    {
    }

    extension(DateTime @this)
    {
        /// <summary>
        /// Deconstructs a DateTime object into its individual components.
        /// </summary>
        /// <param name="this">        The DateTime object to deconstruct. </param>
        /// <param name="year">        The year component of the DateTime. </param>
        /// <param name="month">       The month component of the DateTime. </param>
        /// <param name="day">         The day component of the DateTime. </param>
        /// <param name="hour">        The hour component of the DateTime. </param>
        /// <param name="minute">      The minute component of the DateTime. </param>
        /// <param name="second">      The second component of the DateTime. </param>
        /// <param name="millisecond"> The millisecond component of the DateTime. </param>
        public void Deconstruct(out int year,
                out int month,
                out int day,
                out int hour,
                out int minute,
                out int second,
                out int millisecond) =>
            (year, month, day, hour, minute, second, millisecond) = (@this.Year, @this.Month, @this.Day, @this.Hour, @this.Minute, @this.Second, @this.Millisecond);

        /// <summary>
        /// Deconstructs a DateTime object into its year, month, and day components.
        /// </summary>
        public void Deconstruct(out int year, out int month, out int day) =>
            @this.Deconstruct(out year, out month, out day, out _, out _, out _, out _);

        /// <summary>
        /// Deconstructs a DateTime object into its hour, minute, second, and millisecond components.
        /// </summary>
        public void Deconstruct(out int hour, out int minute, out int second, out int millisecond) =>
            (hour, minute, second, millisecond) = (@this.Hour, @this.Minute, @this.Second, @this.Millisecond);

        public DateOnly GetDateOnly() =>
            DateOnly.FromDateTime(@this);

        public TimeBand GetTimeBand() =>
            @this.Hour switch
            {
                < 6 or > 19 => TimeBand.Overnight,
                < 10 => TimeBand.MorningRush,
                < 16 => TimeBand.Daytime,
                _ => TimeBand.Eveningrush
            };

        public TimeOnly GetTimeOnly() =>
            TimeOnly.FromDateTime(@this);

        public bool IsBetween(in DateTime start, in DateTime end) =>
            @this >= start && @this < end;

        public bool IsWeekend(CultureInfo? culture = null) =>
            (culture ?? CultureInfo.CurrentCulture).GetWeekdayState(@this.ArgumentNotNull().DayOfWeek)
                is CultureInfoHelper.WeekdayState.Weekend or CultureInfoHelper.WeekdayState.WorkdayMorning;

        public PersianDateTime ToPersian() =>
            @this;

        public TimeSpan ToTimeSpan() =>
            new(@this.Ticks);
    }

    extension(DateTime? @this)
    {
    }

    extension(TimeSpan @this)
    {
        /// <summary>
        /// Determines whether this instance start is between the given range.
        /// </summary>
        public bool IsBetween(in TimeSpan start, in TimeSpan end) =>
            @this >= start && @this <= end;

        /// <summary>
        /// Determines whether this instance start is between the specified string range.
        /// </summary>
        public bool IsBetween(in string start, in string end) =>
            @this.IsBetween(ToTimeSpan(start), ToTimeSpan(end));

        /// <summary>
        /// Converts to datetime.
        /// </summary>
        public DateTime ToDateTime() =>
            new(@this.Ticks);
    }

    /// <summary>
    /// Returns true if dateTime format is valid.
    /// </summary>
    /// <param name="dateTime"> The date time. </param>
    /// <returns> <c>true</c> if the specified date time is valid; otherwise, <c>false</c>. </returns>
    public static bool IsValid(in string dateTime) =>
        DateTime.TryParse(dateTime, out _);

    /// <summary>
    /// Converts to timespan.
    /// </summary>
    /// <param name="this"> The source. </param>
    /// <returns> </returns>
    public static TimeSpan ToTimeSpan(in string @this) =>
        TimeSpan.Parse(@this, CultureInfo.CurrentCulture);

}