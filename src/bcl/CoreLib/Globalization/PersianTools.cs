using System.Globalization;

using Library.Globalization.DataTypes;
using Library.Validations;

using static Library.Globalization.CultureConstants;

namespace Library.Globalization;

public static class PersianTools
{
    #region Fields

    public static readonly IEnumerable<char> Chars = "ضصثقفغعهخحجچپسیبلاتنمکگظطزرذدئوژآ";

    public static readonly (char Persian, char Arabic)[] InvalidArabicCharPairs = new[]
    {
            ('ک','ﮎ'),
            ('ک','ﮏ'),
            ('ک','ﮐ'),
            ('ک','ﮑ'),
            ('ک','ك'),
            ('ی','ي'),
            ('ی','ئ'),
            ('ه','ة'),
            ('ه','ۀ'),
            ('و','ؤ')
        };

    public static readonly char[] SeparatorsChars = new[] { ',', DECIMAL_SEPARATOR };
    public static readonly IEnumerable<char> SpecialChars = "ًٌٍريال،؛َُِّۀةيؤإأء";
    private static readonly PersianCalendar _persianCalendar = new();

    #endregion Fields

    /// <summary>
    /// Gets the Persian digits.
    /// </summary>
    /// <value>The Persian digits.</value>
    public static IEnumerable<(char English, char Persian)> Digits
    {
        get
        {
            yield return ('0', NUMBER0);
            yield return ('1', NUMBER1);
            yield return ('2', NUMBER2);
            yield return ('3', NUMBER3);
            yield return ('4', NUMBER4);
            yield return ('5', NUMBER5);
            yield return ('6', NUMBER6);
            yield return ('7', NUMBER7);
            yield return ('8', NUMBER8);
            yield return ('9', NUMBER9);
        }
    }

    public static IEnumerable<char> PersianDigits => Digits.Select(x => x.Persian);

    public static int GetDayCount(this PersianMonth persianMonth, in int? year = null) => persianMonth switch
    {
        PersianMonth.Farvardin => 31,
        PersianMonth.Ordibehesht => 31,
        PersianMonth.Khordad => 31,
        PersianMonth.Tir => 31,
        PersianMonth.Mordad => 31,
        PersianMonth.Sharivar => 31,
        PersianMonth.Mehr => 30,
        PersianMonth.Aban => 30,
        PersianMonth.Azar => 30,
        PersianMonth.Dey => 30,
        PersianMonth.Bahman => 30,
        PersianMonth.Esfand => year.HasValue ? PersianDateTime.PersianCalendar.IsLeapYear(year.Value) ? 30 : 29 : 29,
        _ => 0
    };

    /// <summary>
    /// Gets the name of the month.
    /// </summary>
    /// <param name="month">The month.</param>
    /// <param name="language">The language.</param>
    /// <param name="longName">if set to <c>true</c> [long name].</param>
    /// <returns></returns>
    public static string GetMonthName(in int month, in Language language, in bool longName) => month switch
    {
        1 => language switch
        {
            Language.Persian => longName ? FARVARDIN_FA : FARVARDIN_ABBR_FA,
            Language.English => longName ? FARVARDIN_EN : FARVARDIN_ABBR_EN,
            _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
        },
        2 => language switch
        {
            Language.Persian => longName ? ORDIBEHESHT_FA : ORDIBEHESHT_ABBR_FA,
            Language.English => longName ? ORDIBEHESHT_EN : ORDIBEHESHT_ABBR_EN,
            _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
        },
        3 => language switch
        {
            Language.Persian => longName ? KHORDAD_FA : KHORDAD_ABBR_FA,
            Language.English => longName ? KHORDAD_EN : KHORDAD_ABBR_EN,
            _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
        },
        4 => language switch
        {
            Language.Persian => longName ? TIR_FA : TIR_ABBR_FA,
            Language.English => longName ? TIR_EN : TIR_ABBR_EN,
            _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
        },
        5 => language switch
        {
            Language.Persian => longName ? MORDAD_FA : MORDAD_ABBR_FA,
            Language.English => longName ? MORDAD_EN : MORDAD_ABBR_EN,
            _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
        },
        6 => language switch
        {
            Language.Persian => longName ? SHAHRIVAR_FA : SHAHRIVAR_ABBR_FA,
            Language.English => longName ? SHAHRIVAR_EN : SHAHRIVAR_ABBR_EN,
            _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
        },
        7 => language switch
        {
            Language.Persian => longName ? MEHR_FA : MEHR_ABBR_FA,
            Language.English => longName ? MEHR_EN : MEHR_ABBR_EN,
            _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
        },
        8 => language switch
        {
            Language.Persian => longName ? ABAN_FA : ABAN_ABBR_FA,
            Language.English => longName ? ABAN_EN : ABAN_ABBR_EN,
            _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
        },
        9 => language switch
        {
            Language.Persian => longName ? AZAR_FA : AZAR_ABBR_FA,
            Language.English => longName ? AZAR_EN : AZAR_ABBR_EN,
            _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
        },
        10 => language switch
        {
            Language.Persian => longName ? DEY_FA : DEY_ABBR_FA,
            Language.English => longName ? DEY_EN : DEY_ABBR_EN,
            _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
        },
        11 => language switch
        {
            Language.Persian => longName ? BAHMAN_FA : BAHMAN_ABBR_FA,
            Language.English => longName ? BAHMAN_EN : BAHMAN_ABBR_EN,
            _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
        },
        12 => language switch
        {
            Language.Persian => longName ? ESFAND_FA : ESFAND_ABBR_FA,
            Language.English => longName ? ESFAND_EN : ESFAND_ABBR_EN,
            _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
        },
        _ => throw new ArgumentOutOfRangeException(nameof(month), month, null)
    };

    /// <summary>
    /// Gets the parts.
    /// </summary>
    /// <param name="persianDate">The Persian date.</param>
    /// <returns></returns>
    public static (int year, int month, int day) GetParts(in string persianDate)
    {
        var parts = persianDate.Split('/')
            .Select(s =>
            {
                s = s.Trim();
                if (s.Contains(' '))
                {
                    s = s[..s.IndexOf(' ')];
                }

                return Convert.ToInt32(s);
            })
            .ToArray();
        if (parts.Length == 3)
        {
            return (parts[0], parts[1], parts[2]);
        }

        var result = new int[3];
        result[0] = GetPersianYear(DateTime.Now);
        result[1] = parts[0];
        result[2] = parts[1];
        return (result[0], result[1], result[2]);
    }

    /// <summary>
    /// Gets the Persian date parts.
    /// </summary>
    /// <param name="date">The date.</param>
    /// <returns></returns>
    public static (int Year, int Month, int Day) GetPersianDateParts(in DateTime date)
        => (GetPersianYear(date), GetPersianMonth(date), GetPersianDayOfMonth(date));

    /// <summary>
    /// Gets the Persian day of month.
    /// </summary>
    /// <param name="dateTime">The date time.</param>
    /// <returns></returns>
    public static int GetPersianDayOfMonth(this DateTime dateTime)
        => _persianCalendar.GetDayOfMonth(dateTime);

    /// <summary>
    /// Gets the Persian days count in month.
    /// </summary>
    /// <param name="year">The year.</param>
    /// <param name="month">The month.</param>
    /// <returns></returns>
    public static int GetPersianDaysCountInMonth(in int year, in int month)
        => _persianCalendar.GetDaysInMonth(year, month);

    /// <summary>
    /// Gets the Persian first day of month.
    /// </summary>
    /// <param name="current">The current.</param>
    /// <returns></returns>
    public static DateTime GetPersianFirstDayOfMonth(this DateTime current)
    {
        while (GetPersianDayOfMonth(current) is not 1)
        {
            current = current.AddDays(-1);
        }

        return current;
    }

    /// <summary>
    /// Gets the Persian month.
    /// </summary>
    /// <param name="dateTime">The date time.</param>
    /// <returns></returns>
    public static int GetPersianMonth(this DateTime dateTime)
        => _persianCalendar.GetMonth(dateTime);

    /// <summary>
    /// Gets the Persian year.
    /// </summary>
    /// <param name="dateTime">The date time.</param>
    /// <returns></returns>
    public static int GetPersianYear(this DateTime dateTime)
        => _persianCalendar.GetYear(dateTime);

    public static object GetWeekDayName(DateTime dateTime) 
        => dateTime.DayOfWeek.ToPersianString();

    /// <summary>
    /// Converts to Persian string.
    /// </summary>
    /// <param name="year">The year.</param>
    /// <param name="month">The month.</param>
    /// <param name="day">The day.</param>
    /// <param name="hour">The hour.</param>
    /// <param name="minute">The minute.</param>
    /// <param name="second">The second.</param>
    /// <param name="format">The format.</param>
    /// <param name="language">The language.</param>
    /// <returns></returns>
    public static string PersianDateToPersianString(int? year,
        int? month,
        int? day,
        int? hour = 0,
        int? minute = 0,
        int? second = 0,
        in string format = DEFAULT_DATE_TIME_PATTERN,
        in Language language = Language.Persian)
        => Reformat(year, month, day, hour, minute, second, format, language);

    /// <summary>
    /// Re-formats the Persian string.
    /// </summary>
    /// <param name="persianDate">The Persian date.</param>
    /// <returns></returns>
    public static string ReformatPersian(string persianDate)
    {
        (var year, var month, var day) = GetParts(persianDate);
        return PersianDateToPersianString(year, month, day);
    }

    /// <summary>
    /// Converts to datetime.
    /// </summary>
    /// <param name="persianDateText">The Persian date text.</param>
    /// <returns></returns>
    public static DateTime ToDateTime(in string persianDateText)
    {
        (var year, var month, var day) = GetParts(persianDateText);
        return _persianCalendar.ToDateTime(year, month, day, 0, 0, 0, 0);
    }

    /// <summary>
    /// Converts Persian number to English number.
    /// </summary>
    /// <param name="persianNumber">The Persian number.</param>
    /// <returns></returns>
    public static string ToEnglishNumber(in string persianNumber)
    {
        var chars = new char[persianNumber.Length];
        for (var index = 0; index < persianNumber.Length; index++)
        {
            var c = persianNumber[index];
            (var english, var persian) = Digits.FirstOrDefault(pd => pd.Persian == c);
            var r = persian.ToString() is not null && persian != '\0' ? english.ToString()[0] : c;
            chars[index] = r;
        }

        return new string(chars);
    }

    /// <summary>
    /// Translates to Persian.
    /// </summary>
    /// <param name="day">The day.</param>
    /// <returns></returns>
    public static string ToPersian(DayOfWeek day) =>
        PersianDateTime.PersianWeekDays.FirstOrDefault(d => d.Day == day).Name;

    /// <summary>
    /// Converts datetime to Persian date.
    /// </summary>
    /// <param name="date">The date.</param>
    /// <returns></returns>
    public static string ToPersianDate(this DateTime date)
    {
        (var year, var month, var day) = GetPersianDateParts(date);
        return Reformat(year, month, day, null, null, null, SHORT_DATE_PATTERN, Language.Persian);
    }

    public static PersianDateTime ToPersianDateTime(this DateTime dateTime)
        => new(dateTime);

    /// <summary>
    /// Converts to Persian day of week.
    /// </summary>
    /// <param name="dow">The dow.</param>
    /// <returns></returns>
    public static PersianDayOfWeek ToPersianDayOfWeek(this DayOfWeek dow)
        => (PersianDayOfWeek)dow;

    public static string ToPersianString(this DayOfWeek dow) => dow switch
    {
        DayOfWeek.Sunday => YEK_SHANBE_FA,
        DayOfWeek.Monday => DO_SHANBE_FA,
        DayOfWeek.Tuesday => SE_SHANBE_FA,
        DayOfWeek.Wednesday => CHAHAR_SHANBE_FA,
        DayOfWeek.Thursday => PANJ_SHANBE_FA,
        DayOfWeek.Friday => JOME_FA,
        DayOfWeek.Saturday => SHANBE_FA,
        _ => throw new ArgumentOutOfRangeException(nameof(dow), dow, null)
    };

    private static string Reformat(int? year, int? month, int? day, int? hour, int? minute, int? second, string format, Language language)
    {
        Check.MustBeArgumentNotNull(format);
        Check.MustBe(Enum.IsDefined(typeof(Language), language), () => new ArgumentOutOfRangeException(nameof(language)));

        (var y, var m, var d) = PersianDateTime.Now;
        year ??= y;
        month ??= m;
        day ??= d;
        hour ??= 0;
        minute ??= 0;
        second ??= 0;
        var isAm = hour < 12;

        var result = format
            .Replace("yyyy", year.ToString("0000"), StringComparison.Ordinal)
            .Replace("yy", year > 1300 ? (year - 1300).ToString("00") : year.ToString("00"), StringComparison.Ordinal)
            .Replace("MMMM", GetMonthName(month.Value, language, true), StringComparison.Ordinal)
            .Replace("MM", month.ToString("00"), StringComparison.Ordinal)
            .Replace("dd", day.ToString("00"), StringComparison.Ordinal)
            .Replace("HH", hour.ToString("00"), StringComparison.Ordinal)
            .Replace("hh", isAm ? hour.ToString("00") : (hour - 12).ToString("00"), StringComparison.Ordinal)
            .Replace("mm", minute.ToString("00"), StringComparison.Ordinal)
            .Replace("ss", second.ToString("00"), StringComparison.Ordinal)
            .Replace("ss", language switch
            {
                Language.Persian => isAm ? AM_DESIGNATOR : PM_DESIGNATOR,
                Language.English => isAm ? "AM" : "PM",
                Language.None => throw new NotImplementedException(),
                _ => throw new NotImplementedException()
            }, StringComparison.Ordinal);

        return result;
    }
}