using System.Globalization;

using Library.Globalization.DataTypes;

namespace Library.Globalization.CustomFormatters;

/// <summary>
///     Formats the numbers in specific culture
/// </summary>
/// <seealso cref="System.IFormatProvider" />
/// <seealso cref="System.ICustomFormatter" />
[Serializable]
public sealed class CustomNumberFormatter : IFormatProvider, ICustomFormatter
{
    public CustomNumberFormatter(NumericFormatInfo numericFormatInfo)
        : this((Thread.CurrentThread.CurrentCulture.NumberFormat.Clone()! as NumberFormatInfo)!, numericFormatInfo)
    {
    }

    public CustomNumberFormatter(NumberFormatInfo numberFormatInfo, NumericFormatInfo numericFormatInfo)
    {
        this.Engine = numberFormatInfo;
        this.Info = numericFormatInfo;
        this.Info.MapToNumberFormatInfo(this.Engine);
    }

    /// <summary>
    ///     Gets the engine.
    /// </summary>
    /// <value>
    ///     The engine.
    /// </value>
    public NumberFormatInfo Engine { get; }

    /// <summary>
    ///     Gets the information.
    /// </summary>
    /// <value>
    ///     The information.
    /// </value>
    public NumericFormatInfo Info { get; }

    /// <summary>
    ///     Converts the value of a specified object to an equivalent string representation using specified format and
    ///     culture-specific formatting information.
    /// </summary>
    /// <param name="format">A format string containing formatting specifications.</param>
    /// <param name="arg">An object to format.</param>
    /// <param name="formatProvider">An object that supplies format information about the current instance.</param>
    /// <returns>
    ///     The string representation of the value of <paramref name="arg" />, formatted as specified by
    ///     <paramref name="format" /> and <paramref name="formatProvider" />.
    /// </returns>
    public string Format(string? format, object? arg, IFormatProvider? formatProvider)
    {
        var buffer = format?.ToUpper();
        if (buffer is null or not "CO")
        {
            return string.Format(this.Engine, "{{0:" + format + "}}", arg);
        }

        if (this.Engine.Clone() is not NumberFormatInfo temp)
        {
            return string.Empty;
        }

        temp.CurrencyDecimalDigits = this.Info.Cost.DecimalDigits;
        temp.CurrencyDecimalSeparator = this.Info.Cost.DecimalSeparator.ToString();
        temp.CurrencyGroupSeparator = this.Info.Cost.GroupSeparator.ToString();
        temp.CurrencyNegativePattern = this.Info.Cost.NegativePattern;
        temp.CurrencyPositivePattern = this.Info.Cost.PositivePattern;
        temp.CurrencySymbol = this.Info.Cost.Symbol;
        return string.Format(temp, "{0:C}", arg);
    }

    /// <summary>
    ///     Returns an object that provides formatting services for the specified type.
    /// </summary>
    /// <param name="formatType">An object that specifies the type of format object to return.</param>
    /// <returns>
    ///     An instance of the object specified by <paramref name="formatType" />, if the
    ///     <see cref="T:System.IFormatProvider" /> implementation can supply that type of object; otherwise,
    ///     <see langword="null" />.
    /// </returns>
    public object? GetFormat(Type? formatType) => formatType == typeof(ICustomFormatter) ? this : null;

    /// <summary>
    ///     Parses the cost.
    /// </summary>
    /// <param name="formattedNumber">The formatted number.</param>
    /// <returns></returns>
    public double ParseCost(string formattedNumber)
    {
        if (this.Engine.Clone() is not NumberFormatInfo temp)
        {
            return 0.0;
        }

        temp.CurrencyDecimalDigits = this.Info.Cost.DecimalDigits;
        temp.CurrencyDecimalSeparator = this.Info.Cost.DecimalSeparator.ToString();
        temp.CurrencyGroupSeparator = this.Info.Cost.GroupSeparator.ToString();
        temp.CurrencyNegativePattern = this.Info.Cost.NegativePattern;
        temp.CurrencyPositivePattern = this.Info.Cost.PositivePattern;
        temp.CurrencySymbol = this.Info.Cost.Symbol;
        return double.Parse(formattedNumber, NumberStyles.Any, temp);
    }
}
