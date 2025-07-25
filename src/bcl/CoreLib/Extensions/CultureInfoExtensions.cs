using System.Globalization;

using Library.Globalization;

namespace Library.Extensions;

public static class CultureInfoExtension
{
    private static readonly char[] _dash = ['-'];

    private static readonly char[] _separator = ['(', ')'];

    private static readonly char[] _separatorArray = [','];

    extension(CultureInfo @this)
    {
        /// <summary>
        /// Returns the English version of the country name. Extracted from the CultureInfo.EnglishName.
        /// </summary>
        /// <param name="this"> The CultureInfo this object. </param>
        /// <returns> The English version of the country name. </returns>
        public string GetCountryEnglishName()
        //This code is used to get the English name of a country from a CultureInfo object.
        {
            Check.MustBeArgumentNotNull(@this);
            //Split the EnglishName property of the CultureInfo object into an array of strings, removing any empty entries
            var parts = @this.EnglishName.Split(_separator, StringSplitOptions.RemoveEmptyEntries);
            //If the array has fewer than two elements, return the EnglishName property
            if (parts.Length < 2)
            {
                return @this.EnglishName;
            }

            //Split the second element of the array into an array of strings, removing any empty entries
            parts = parts[1].Split(_separatorArray, StringSplitOptions.RemoveEmptyEntries);
            //Return the last element of the array, trimmed of any whitespace
            return parts[^1].Trim();
        }

        /// <summary>
        /// Returns the English version of the language name. Extracted from the CultureInfo.EnglishName.
        /// </summary>
        /// <param name="this"> The CultureInfo this object. </param>
        /// <returns> The English version of the language name. </returns>
        public string GetLanguageEnglishName()
            => @this.EnsureArgumentNotNull().EnglishName.Split(['('], StringSplitOptions.RemoveEmptyEntries)[0].Trim();

        /// <summary>
        /// Gets the weekday state for the given culture and day of week.
        /// </summary>
        /// <param name="this"> The culture info. </param>
        /// <param name="day">  The day of week. </param>
        /// <returns> The weekday state. </returns>
        public WeekdayState GetWeekdayState(DayOfWeek day)
            => GetCountryAbbreviation(@this.EnsureArgumentNotNull()) switch
            {
                "DZ" // Algeria
                or "BH" // Bahrain
                or "BD" // Bangladesh
                or "EG" // Egypt
                or "IQ" // Iraq
                or "IL" // Israel
                or "JO" // Jordan
                or "KW" // Kuwait
                or "LY" // Libya
                        // Northern Malaysia (only in the states of Kelantan, Terengganu, and Kedah)
                or "MV" // Maldives
                or "MR" // Mauritania
                or "NP" // Nepal
                or "OM" // Oman
                or "QA" // Qatar
                or "SA" // Saudi Arabia
                or "SD" // Sudan
                or "SY" // Syria
                or "AE" // U.A.E.
                or "YE" // Yemen
                    => day is DayOfWeek.Thursday or DayOfWeek.Friday
                         ? WeekdayState.Weekend
                         : WeekdayState.Workday,
                "AF" when day is DayOfWeek.Thursday or DayOfWeek.Friday // Afghanistan
                    => WeekdayState.Weekend,
                "AF" // Afghanistan
                    => WeekdayState.Workday,
                "IR" when day is DayOfWeek.Thursday or DayOfWeek.Friday // Iran
                    => WeekdayState.Weekend,
                "IR" // Iran
                    => WeekdayState.Workday,
                "BN" // Brunei Darussalam
                    => day is DayOfWeek.Friday or DayOfWeek.Sunday
                            ? WeekdayState.Weekend
                            : WeekdayState.Workday,
                "MX" // Mexico
                or "TH" when day == DayOfWeek.Saturday // Thailand
                    => WeekdayState.WorkdayMorning,
                "TH" // Thailand
                    => day is DayOfWeek.Saturday or DayOfWeek.Sunday
                            ? WeekdayState.Weekend
                            : WeekdayState.Workday,
                _ => day is DayOfWeek.Saturday or DayOfWeek.Sunday ? WeekdayState.Weekend : WeekdayState.Workday
            };

        /// <summary>
        /// Determines whether the specified day of the week is a weekend.
        /// </summary>
        /// <param name="dayOfWeek">The day of the week to evaluate.</param>
        /// <returns><see langword="true"/> if the specified day is a weekend; otherwise, <see langword="false"/>.</returns>
        public bool IsWeekend(DayOfWeek dayOfWeek)
            => GetWeekdayState(@this, dayOfWeek) == WeekdayState.Weekend;
    }

    /// <summary>
    /// Gets the country abbreviation from a CultureInfo object.
    /// </summary>
    /// <param name="this"> The CultureInfo object. </param>
    /// <returns> The country abbreviation. </returns>
    private static string GetCountryAbbreviation(CultureInfo @this)
        => @this.Name.Split(_dash, StringSplitOptions.RemoveEmptyEntries)[^1];

    /// <summary>
    /// The weekday/weekend state for a given day.
    /// </summary>
}