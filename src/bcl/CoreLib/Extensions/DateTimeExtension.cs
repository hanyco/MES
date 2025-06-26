using System.Globalization;

using Library.Exceptions;
using Library.Globalization;

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
                is WeekdayState.Weekend or WeekdayState.WorkdayMorning;

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
            @this.IsBetween(TimeSpan.Parse(start), TimeSpan.Parse(end));

        /// <summary>
        /// Converts to datetime.
        /// </summary>
        public DateTime ToDateTime() =>
            new(@this.Ticks);
    }
}