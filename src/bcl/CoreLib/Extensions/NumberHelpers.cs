using Library.Exceptions;
using Library.Globalization;
using Library.Validations;

namespace Library.Helpers;

public static class NumberHelper
{
    private static readonly string[] _sizeSuffixes = { "", "K", "M", "G", "T", "P", "E", "Z", "Y" };

    [Obsolete($"Use `System.Nullable<T>.GetValueOrDefault()` instead.", true)]
    public static long IfNull(this long? value, long defaultValue) =>
        value ?? defaultValue;

    /// <summary>
    /// Checks if a given integer is between two other integers.
    /// </summary>
    public static bool IsBetween(this int num, in int min, in int max) =>
        num >= min && num <= max;

    public static bool IsNullOrZero(this int? number) =>
        number is null or 0;

    public static bool IsNullOrZero(params int?[] numbers) =>
        numbers.All(IsNullOrZero);

    /// <summary>
    /// Checks if a given number is a prime number.
    /// </summary>
    /// <param name="number">The number to check.</param>
    /// <returns>True if the number is a prime number, false otherwise.</returns>
    public static bool IsPrime(int number)
    {
        //Check if number is less than or equal to 1, if so return false
        if (number <= 1)
        {
            return false;
        }
        //Check if number is equal to 2, if so return true
        if (number == 2)
        {
            return true;
        }
        //Check if number is divisible by 2, if so return false
        if (number % 2 == 0)
        {
            return false;
        }

        //Loop through all numbers from 3 to the square root of the given number
        for (var i = 3; i <= Math.Sqrt(number); i += 2)
        {
            //Check if the given number is divisible by the current number in the loop, if so return false
            if (number % i == 0)
            {
                return false;
            }
        }

        //If the number passes all the checks, return true
        return true;
    }

    /// <summary>
    /// Generates a random number between the given min and max values. If no min or max values are
    /// provided, the default min and max values are used.
    /// </summary>
    public static int RandomNumber(int? min = null, int? max = null) =>
        InnerRandomizerMethod(min, max)();

    /// <summary>
    /// Generates a sequence of random numbers within a specified range.
    /// </summary>
    /// <param name="count">The number of random numbers to generate.</param>
    /// <param name="min">The minimum value of the random numbers.</param>
    /// <param name="max">The maximum value of the random numbers.</param>
    /// <returns>A sequence of random numbers within the specified range.</returns>
    public static IEnumerable<int> RandomNumbers(int count, int? min = null, int? max = null) =>
        Range(1, count).Select(_ => InnerRandomizerMethod(min, max)());

    /// <summary>
    /// Generates a sequence of integers within a specified range.
    /// </summary>
    /// <param name="start">The start of the range.</param>
    /// <param name="end">The end of the range.</param>
    /// <param name="step">The step between each value in the range.</param>
    /// <returns>A sequence of integers within the specified range.</returns>
    public static IEnumerable<int> Range(int start, int end, int step = 1)
    {
        // Check if step is not equal to 0
        Check.MustBe(step != 0, () => new ArgumentOutOfRangeException(nameof(step)));
        // Throw exception if step is positive and start is greater than end
        if (step > 0 && start > end)
        {
            throw new InvalidArgumentException(nameof(step));
        }
        // Throw exception if step is negative and start is less than end
        if (step < 0 && start < end)
        {
            throw new InvalidArgumentException(nameof(step));
        }

        // Create a function to check if the end condition is met
        Func<int, bool> endCondition = step > 0 ? i => i <= end : i => i >= end;

        // Loop through the range and yield the values
        for (var i = start; endCondition(i); i += step)
        {
            yield return i;
        }
    }
    
    /// <summary>
    /// Converts a string to a cultural number based on the specified language and corrects Persian
    /// characters if needed.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <param name="correctPersianChars">
    /// A boolean value indicating whether Persian characters should be corrected.
    /// </param>
    /// <param name="toLanguage">The language to convert the string to.</param>
    /// <returns>The converted string.</returns>
    [return: NotNullIfNotNull(nameof(value))]
    public static string ToCulturalNumber(in string value, in Language toLanguage) =>
        toLanguage switch
        {
            Language.Persian => ToPersianDigits(value),
            Language.English => ToEnglishDigits(value),
            _ => value
        };

    /// <summary>
    /// Replaces all Persian digits in the given string with their English equivalents.
    /// </summary>
    [return: NotNullIfNotNull(nameof(value))]
    public static string ToEnglishDigits(this string value) =>
        value.ReplaceAll(PersianTools.Digits.Select(n => (n.Persian, n.English)));

    /// <summary>
    /// Converts an integer to its Persian equivalent.
    /// </summary>
    /// <param name="number">The number to convert.</param>
    /// <returns>The Persian equivalent of the number.</returns>
    public static string ToPersian(int number)
    {
        var persianNumbers = PersianTools.PersianDigits.Select(x => x.ToString()).ToArray();
        var result = "";
        while (number > 0)
        {
            result = persianNumbers[number % 10] + result;
            number /= 10;
        }
        return result;
    }

    /// <summary>
    /// Replaces all English digits in the given string with their Persian equivalents.
    /// </summary>
    [return: NotNullIfNotNull(nameof(value))]
    public static string ToPersianDigits(in string value) =>
        value.ReplaceAll(PersianTools.Digits.Select(n => (n.English, n.Persian)));

    /// <summary>
    /// Replaces all English digits in the given string with their Persian equivalents.
    /// </summary>
    public static string ToPersianDigits(in int value) =>
        value.ToString().ReplaceAll(PersianTools.Digits.Select(n => (n.English, n.Persian)));

    /// <summary>
    /// Converts a long value to a standard metric scale.
    /// </summary>
    /// <param name="value">The value to be converted.</param>
    /// <param name="measure">The measure to use for conversion (defaults to 1000).</param>
    /// <param name="decimalPlaces">The number of decimal places to use (defaults to 1).</param>
    /// <returns>A string with the converted value and the corresponding size suffix.</returns>
    public static string ToStandardMetricScale(this long value, int measure = 1000, int decimalPlaces = 1)
    //This code converts a long value to a standard metric scale.
    //It takes in two parameters, measure and decimalPlaces, which are set to 1000 and 1 respectively by default.
    //It first checks if the value is negative, and if so, it calls the same function with the negative value.
    //If the measure is 1, it returns the value as a string.
    //Otherwise, it divides the value by the measure and increments the index until the rounded value is less than the measure.
    //Finally, it returns a string with the value and the corresponding size suffix.
    {
        //If the value is less than 0, convert it to a positive value and call the ToStandardMetricScale function with the positive value, measure, and decimalPlaces as parameters.
        if (value < 0)
        {
            return "-" + ToStandardMetricScale(-value, measure, decimalPlaces);
        }

        //If the measure is equal to 1, return the value as a string.
        if (measure == 1)
        {
            return value.ToString();
        }

        //Declare a variable to keep track of the index and set it to 0.
        //Declare a variable to store the decimal value of the value and set it to the value.
        var i = 0;
        decimal dValue = value;

        //While the rounded decimal value is greater than or equal to the measure, divide the decimal value by the measure and increment the index.
        while (Math.Round(dValue, decimalPlaces) >= measure)
        {
            dValue /= measure;
            i++;
        }

        //Return a formatted string with the decimal value, the number of decimal places, and the size suffix at the index.
        return string.Format("{0:n" + decimalPlaces + "} {1}", dValue, _sizeSuffixes[i]);
    }

    /// <summary>
    /// Converts an integer to a string using the specified format and default value.
    /// </summary>
    public static string ToString(this int? number, string format = "0", int defaultValue = 0) =>
        (number ?? defaultValue).ToString(format);

    private static Func<int> InnerRandomizerMethod(int? min, int? max) =>
        (min, max) switch
        {
            (null, null) => Random.Shared.Next,
            (null, not null) => () => Random.Shared.Next(max.Value),
            (not null, not null) => () => Random.Shared.Next(min.Value, max.Value),
            (not null, null) => () => Random.Shared.Next(min.Value, int.MaxValue)
        };
}