using Library.Globalization.DataTypes;

namespace Library.Helpers;

/// <summary>
/// Provides helper methods for converting between PersianDayOfWeek and DayOfWeek, and for checking
/// if a PersianDayOfWeek is a holiday.
/// </summary>
/// <returns>
/// Methods for converting between PersianDayOfWeek and DayOfWeek, and for checking if a
/// PersianDayOfWeek is a holiday.
/// </returns>
public static class GlobalizationExtensions
{
    /// <summary>
    /// Checks if the given PersianDayOfWeek is a holiday.
    /// </summary>
    /// <param name="dow">The PersianDayOfWeek to check.</param>
    /// <returns>True if the given PersianDayOfWeek is a holiday, false otherwise.</returns>
    public static bool IsPersianHoliday(this PersianDayOfWeek dow) =>
        dow switch
        {
            PersianDayOfWeek.Jomeh or PersianDayOfWeek.Shanbeh => true,
            PersianDayOfWeek.YekShanbeh => false,
            PersianDayOfWeek.DoShanbeh => false,
            PersianDayOfWeek.SeShanbeh => false,
            PersianDayOfWeek.ChaharShanbeh => false,
            PersianDayOfWeek.PanjShanbeh => false,
            _ => false,
        };

    /// <summary>
    /// Converts a PersianDayOfWeek to a DayOfWeek.
    /// </summary>
    /// <param name="dow">The PersianDayOfWeek to convert.</param>
    /// <returns>The DayOfWeek corresponding to the given PersianDayOfWeek.</returns>
    public static DayOfWeek ToDayOfWeek(this PersianDayOfWeek dow) =>
        dow switch
        {
            PersianDayOfWeek.YekShanbeh => DayOfWeek.Sunday,
            PersianDayOfWeek.DoShanbeh => DayOfWeek.Monday,
            PersianDayOfWeek.SeShanbeh => DayOfWeek.Tuesday,
            PersianDayOfWeek.ChaharShanbeh => DayOfWeek.Wednesday,
            PersianDayOfWeek.PanjShanbeh => DayOfWeek.Thursday,
            PersianDayOfWeek.Jomeh => DayOfWeek.Friday,
            PersianDayOfWeek.Shanbeh => DayOfWeek.Saturday,
            _ => throw new NotImplementedException(),
        };

    /// <summary>
    /// Converts a DayOfWeek to a PersianDayOfWeek.
    /// </summary>
    /// <param name="dow">The DayOfWeek to convert.</param>
    /// <returns>The PersianDayOfWeek corresponding to the given DayOfWeek.</returns>
    public static PersianDayOfWeek ToPersianDayOfWeek(this DayOfWeek dow) =>
        dow switch
        {
            DayOfWeek.Sunday => PersianDayOfWeek.YekShanbeh,
            DayOfWeek.Monday => PersianDayOfWeek.DoShanbeh,
            DayOfWeek.Tuesday => PersianDayOfWeek.SeShanbeh,
            DayOfWeek.Wednesday => PersianDayOfWeek.ChaharShanbeh,
            DayOfWeek.Thursday => PersianDayOfWeek.PanjShanbeh,
            DayOfWeek.Friday => PersianDayOfWeek.Jomeh,
            DayOfWeek.Saturday => PersianDayOfWeek.Shanbeh,
            _ => throw new NotImplementedException(),
        };
}