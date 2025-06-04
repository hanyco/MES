using System.ComponentModel;
using System.Globalization;

using Library.DesignPatterns.Markers;
using Library.Globalization.Attributes;
using Library.Validations;

namespace Library.Globalization.DataTypes;

public interface INumericFormat
{
    /// <summary>
    /// Gets or sets the decimal digits.
    /// </summary>
    /// <value>The decimal digits.</value>
    int DecimalDigits { get; set; }

    /// <summary>
    /// Gets or sets the decimal separator.
    /// </summary>
    /// <value>The decimal separator.</value>
    char DecimalSeparator { get; set; }

    /// <summary>
    /// Gets the format string.
    /// </summary>
    /// <value>The format string.</value>
    string FormatString { get; }

    /// <summary>
    /// Gets or sets the group separator.
    /// </summary>
    /// <value>The group separator.</value>
    char GroupSeparator { get; set; }

    /// <summary>
    /// Gets or sets the negative pattern.
    /// </summary>
    /// <value>The negative pattern.</value>
    int NegativePattern { get; set; }
}

[Serializable]
public sealed class CostFormat : NumericFormatBase
{
    /// <summary>
    /// The default decimal digits
    /// </summary>
    public static readonly int DefaultDecimalDigits = 8;

    /// <summary>
    /// The default decimal separator
    /// </summary>
    public static readonly char DefaultDecimalSeparator = '.';

    /// <summary>
    /// The default group separator
    /// </summary>
    public static readonly char DefaultGroupSeparator = ',';

    /// <summary>
    /// The default negative pattern
    /// </summary>
    public static readonly int DefaultNegativePattern = CurrencyNegativePattern.Value5.Cast().ToInt();

    /// <summary>
    /// The default positive pattern
    /// </summary>
    public static readonly int DefaultPositivePattern = CurrencyPositivePattern.Value3.Cast().ToInt();

    /// <summary>
    /// The default symbol
    /// </summary>
    public static readonly string DefaultSymbol = "Rls";

    /// <summary>
    /// Initializes a new instance of the <see cref="CostFormat"/> class.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="positivePattern">The positive pattern.</param>
    public CostFormat(string? symbol, int positivePattern)
    {
        this.Symbol = symbol ?? DefaultSymbol;
        this.PositivePattern = positivePattern;
    }

    /// <summary>
    /// Gets the format string.
    /// </summary>
    /// <value>The format string.</value>
    public override string FormatString => "Co";

    /// <summary>
    /// Gets or sets the positive pattern.
    /// </summary>
    /// <value>The positive pattern.</value>
    public int PositivePattern { get; set; }

    /// <summary>
    /// Gets or sets the symbol.
    /// </summary>
    /// <value>The symbol.</value>
    public string Symbol { get; set; }
}

[Serializable]
public sealed class CurrencyFormat : NumericFormatBase
{
    /// <summary>
    /// The default decimal digits
    /// </summary>
    public static readonly int DefaultDecimalDigits = 2;

    /// <summary>
    /// The default decimal separator
    /// </summary>
    public static readonly char DefaultDecimalSeparator = '.';

    /// <summary>
    /// The default group separator
    /// </summary>
    public static readonly char DefaultGroupSeparator = ',';

    /// <summary>
    /// The default negative pattern
    /// </summary>
    public static readonly int DefaultNegativePattern = CurrencyNegativePattern.Value5.Cast().ToInt();

    /// <summary>
    /// The default positive pattern
    /// </summary>
    public static readonly int DefaultPositivePattern = CurrencyPositivePattern.Value1.Cast().ToInt();

    /// <summary>
    /// The default symbol
    /// </summary>
    public static readonly string DefaultSymbol = "Rls";

    /// <summary>
    /// Initializes a new instance of the <see cref="CurrencyFormat"/> class.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <param name="positivePattern">The positive pattern.</param>
    public CurrencyFormat(string? symbol, int positivePattern)
    {
        this.Symbol = symbol ?? DefaultSymbol;
        this.PositivePattern = positivePattern;
    }

    /// <summary>
    /// Gets the format string.
    /// </summary>
    /// <value>The format string.</value>
    public override string FormatString => "C";

    /// <summary>
    /// Gets or sets the positive pattern.
    /// </summary>
    /// <value>The positive pattern.</value>
    public int PositivePattern { get; set; }

    /// <summary>
    /// Gets the symbol.
    /// </summary>
    /// <value>The symbol.</value>
    public string Symbol { get; }
}

[Serializable]
public sealed class NumberFormat : NumericFormatBase
{
    /// <summary>
    /// The default decimal digits
    /// </summary>
    public static readonly int DefaultDecimalDigits = 2;

    /// <summary>
    /// The default decimal separator
    /// </summary>
    public static readonly char DefaultDecimalSeparator = '.';

    /// <summary>
    /// The default group separator
    /// </summary>
    public static readonly char DefaultGroupSeparator = ',';

    /// <summary>
    /// The default negative pattern
    /// </summary>
    public static readonly int DefaultNegativePattern = NumberNegativePattern.Value1.Cast().ToInt();

    /// <summary>
    /// The default negative sign
    /// </summary>
    public static readonly string DefaultNegativeSign = "-";

    /// <summary>
    /// The default positive sign
    /// </summary>
    public static readonly string DefaultPositiveSign = "+";

    /// <summary>
    /// Gets the format string.
    /// </summary>
    /// <value>The format string.</value>
    public override string FormatString => "N";

    /// <summary>
    /// Gets or sets the negative sign.
    /// </summary>
    /// <value>The negative sign.</value>
    public string? NegativeSign { get; set; }

    /// <summary>
    /// Gets or sets the positive sign.
    /// </summary>
    /// <value>The positive sign.</value>
    public string? PositiveSign { get; set; }
}

/// <summary>
/// </summary>
/// <seealso cref="HanyCo.Mes20.Infra.Globalization.DataTypes.INumericFormat"/>
[Serializable]
public abstract class NumericFormatBase : INumericFormat
{
    /// <summary>
    /// Gets or sets the decimal digits.
    /// </summary>
    /// <value>The decimal digits.</value>
    public int DecimalDigits { get; set; }

    /// <summary>
    /// Gets or sets the decimal separator.
    /// </summary>
    /// <value>The decimal separator.</value>
    public char DecimalSeparator { get; set; }

    /// <summary>
    /// Gets the format string.
    /// </summary>
    /// <value>The format string.</value>
    public abstract string FormatString { get; }

    /// <summary>
    /// Gets or sets the group separator.
    /// </summary>
    /// <value>The group separator.</value>
    public char GroupSeparator { get; set; }

    /// <summary>
    /// Gets or sets the negative pattern.
    /// </summary>
    /// <value>The negative pattern.</value>
    public int NegativePattern { get; set; }
}

[Serializable]
public sealed class NumericFormatInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NumericFormatInfo"/> class.
    /// </summary>
    /// <param name="number">The number.</param>
    /// <param name="percent">The percent.</param>
    /// <param name="currency">The currency.</param>
    /// <param name="cost">The cost.</param>
    public NumericFormatInfo(NumberFormat number, PercentFormat percent, CurrencyFormat currency, CostFormat cost)
    {
        this.Number = number;
        this.Percent = percent;
        this.Currency = currency;
        this.Cost = cost;
    }

    /// <summary>
    /// Gets or sets the cost.
    /// </summary>
    /// <value>The cost.</value>
    public CostFormat Cost { get; set; }

    /// <summary>
    /// Gets the currency.
    /// </summary>
    /// <value>The currency.</value>
    public CurrencyFormat Currency { get; }

    /// <summary>
    /// Gets the number.
    /// </summary>
    /// <value>The number.</value>
    public NumberFormat Number { get; }

    /// <summary>
    /// Gets the percent.
    /// </summary>
    /// <value>The percent.</value>
    public PercentFormat Percent { get; }

    /// <summary>
    /// Maps to number format information.
    /// </summary>
    /// <param name="numericFormatInfo">The numeric format information.</param>
    /// <returns></returns>
    public static NumberFormatInfo MapToNumberFormatInfo(NumericFormatInfo numericFormatInfo)
    {
        var result = new NumberFormatInfo();
        MapToNumberFormatInfo(result, numericFormatInfo);
        return result;
    }

    /// <summary>
    /// Maps to number format information.
    /// </summary>
    /// <param name="numberFormatInfo">The number format information.</param>
    /// <param name="numericFormatInfo">The numeric format information.</param>
    /// <exception cref="ArgumentNullException">numericFormatInfo</exception>
    public static void MapToNumberFormatInfo(NumberFormatInfo numberFormatInfo, NumericFormatInfo numericFormatInfo)
    {
        Check.MustBeArgumentNotNull(numericFormatInfo, nameof(numericFormatInfo));

        numberFormatInfo.CurrencyDecimalDigits = numericFormatInfo.Currency.DecimalDigits;
        numberFormatInfo.CurrencyGroupSeparator = numericFormatInfo.Currency.GroupSeparator.ToString();
        numberFormatInfo.CurrencySymbol = numericFormatInfo.Currency.Symbol;
        numberFormatInfo.CurrencyDecimalSeparator = numericFormatInfo.Currency.DecimalSeparator.ToString();
        numberFormatInfo.CurrencyNegativePattern = numericFormatInfo.Currency.NegativePattern;
        numberFormatInfo.NumberNegativePattern = numericFormatInfo.Number.NegativePattern;
        numberFormatInfo.NumberDecimalDigits = numericFormatInfo.Number.DecimalDigits;
        numberFormatInfo.NumberDecimalSeparator = numericFormatInfo.Number.DecimalSeparator.ToString();
        numberFormatInfo.NumberGroupSeparator = numericFormatInfo.Number.GroupSeparator.ToString();
        numberFormatInfo.PercentDecimalDigits = numericFormatInfo.Percent.DecimalDigits;
        numberFormatInfo.PercentDecimalSeparator = numericFormatInfo.Percent.DecimalSeparator.ToString();
        numberFormatInfo.PercentNegativePattern = numericFormatInfo.Percent.NegativePattern;
        numberFormatInfo.PercentSymbol = numericFormatInfo.Percent.Symbol ?? "%";
        numberFormatInfo.PercentGroupSeparator = numericFormatInfo.Percent.GroupSeparator.ToString();
        numberFormatInfo.NegativeSign = numericFormatInfo.Number.NegativeSign ?? "-";
        numberFormatInfo.PositiveSign = numericFormatInfo.Number.PositiveSign ?? "+";
    }

    /// <summary>
    /// Maps to number format information.
    /// </summary>
    /// <returns></returns>
    public NumberFormatInfo MapToNumberFormatInfo()
    {
        var result = new NumberFormatInfo();
        MapToNumberFormatInfo(result, this);
        return result;
    }

    /// <summary>
    /// Maps to number format information.
    /// </summary>
    /// <param name="numberFormatInfo">The number format information.</param>
    public void MapToNumberFormatInfo(NumberFormatInfo numberFormatInfo)
        => MapToNumberFormatInfo(numberFormatInfo, this);
}

[Serializable]
public sealed class PercentFormat : NumericFormatBase
{
    /// <summary>
    /// The default decimal digits
    /// </summary>
    public static readonly int DefaultDecimalDigits = 2;

    /// <summary>
    /// The default decimal separator
    /// </summary>
    public static readonly char DefaultDecimalSeparator = '.';

    /// <summary>
    /// The default group separator
    /// </summary>
    public static readonly char DefaultGroupSeparator = ',';

    /// <summary>
    /// The default negative pattern
    /// </summary>
    public static readonly int DefaultNegativePattern = PercentNegativePattern.Value1.Cast().ToInt();

    /// <summary>
    /// The default positive pattern
    /// </summary>
    public static readonly int DefaultPositivePattern = PercentPositivePattern.Value1.Cast().ToInt();

    /// <summary>
    /// The default symbol
    /// </summary>
    public static readonly string DefaultSymbol = "";

    /// <summary>
    /// Gets the format string.
    /// </summary>
    /// <value>The format string.</value>
    public override string FormatString => "P";

    /// <summary>
    /// Gets or sets the positive pattern.
    /// </summary>
    /// <value>The positive pattern.</value>
    public int PositivePattern { get; set; }

    /// <summary>
    /// Gets or sets the symbol.
    /// </summary>
    /// <value>The symbol.</value>
    public string? Symbol { get; set; }
}

public enum CurrencyNegativePattern
{
    /// <summary>
    /// $n
    /// </summary>
    [Description("($n)")]
    Value0 = 0,

    /// <summary>
    /// -$n
    /// </summary>
    [Description("-$n")]
    Value1 = 1,

    /// <summary>
    /// $-n
    /// </summary>
    [Description("$-n")]
    Value2 = 2,

    /// <summary>
    /// $n-
    /// </summary>
    [Description("$n-")]
    Value3 = 3,

    /// <summary>
    /// (n$)
    /// </summary>
    [Description("(n$)")]
    Value4 = 4,

    /// <summary>
    /// -n$
    /// </summary>
    [Description("-n$")]
    Value5 = 5,

    /// <summary>
    /// n-$
    /// </summary>
    [Description("n-$")]
    Value6 = 6,

    /// <summary>
    /// n$-
    /// </summary>
    [Description("n$-")]
    Value7 = 7,

    /// <summary>
    /// -n $
    /// </summary>
    [Description("-n $")]
    Value8 = 8,

    /// <summary>
    /// -$ n
    /// </summary>
    [Description("-$ n")]
    Value9 = 9,

    /// <summary>
    /// n $-
    /// </summary>
    [Description("n $-")]
    Value10 = 10,

    /// <summary>
    /// $ n-
    /// </summary>
    [Description("$ n-")]
    Value11 = 11,

    /// <summary>
    /// $ -n
    /// </summary>
    [Description("$ -n")]
    Value12 = 12,

    /// <summary>
    /// n- $
    /// </summary>
    [Description("n- $")]
    Value13 = 13,

    /// <summary>
    /// ($ n)
    /// </summary>
    [Description("($ n)")]
    Value14 = 14,

    /// <summary>
    /// (n $)
    /// </summary>
    [Description("(n $)")]
    Value15 = 15
}

public enum CurrencyPositivePattern
{
    /// <summary>
    /// $n
    /// </summary>
    [Description("$n")]
    Value0 = 0,

    /// <summary>
    /// n$
    /// </summary>
    [Description("n$")]
    Value1 = 1,

    /// <summary>
    /// $ n
    /// </summary>
    [Description("$ n")]
    Value2 = 2,

    /// <summary>
    /// n $
    /// </summary>
    [Description("n $")]
    Value3 = 3
}

public enum FormatOption
{
    Number,
    Currency,
    Percent,
    Cost
}

public enum NumberNegativePattern
{
    /// <summary>
    /// (n)
    /// </summary>
    [Description("(n)")]
    Value0 = 0,

    /// <summary>
    /// -n
    /// </summary>
    [Description("-n")]
    Value1 = 1,

    /// <summary>
    /// - n
    /// </summary>
    [Description("- n")]
    Value2 = 2,

    /// <summary>
    /// n-
    /// </summary>
    [Description("n-")]
    Value3 = 3,

    /// <summary>
    /// n -
    /// </summary>
    [Description("n -")]
    Value4 = 4
}

public enum PercentNegativePattern
{
    /// <summary>
    /// -n %
    /// </summary>
    [Description("-n %")]
    Value0 = 0,

    /// <summary>
    /// -n%
    /// </summary>
    [Description("-n%")]
    Value1 = 1
}

/// <summary>
/// Enum for Persian days of the week.
/// </summary>
/// <param name=""></param>
/// <returns>YekShanbeh, DoShanbeh, SeShanbeh, ChaharShanbeh, PanjShanbeh, Jomeh, Shanbeh</returns>
public enum PersianDayOfWeek
{
    [LocalizedDescription("fa-IR", CultureConstants.YEK_SHANBE_FA)]
    [LocalizedDescription("en-US", CultureConstants.YEK_SHANBE_EN)]
    YekShanbeh,

    [LocalizedDescription("fa-IR", CultureConstants.DO_SHANBE_FA)]
    [LocalizedDescription("en-US", CultureConstants.DO_SHANBE_EN)]
    DoShanbeh,

    [LocalizedDescription("fa-IR", CultureConstants.SE_SHANBE_FA)]
    [LocalizedDescription("en-US", CultureConstants.SE_SHANBE_EN)]
    SeShanbeh,

    [LocalizedDescription("fa-IR", CultureConstants.CHAHAR_SHANBE_FA)]
    [LocalizedDescription("en-US", CultureConstants.CHAHAR_SHANBE_EN)]
    ChaharShanbeh,

    [LocalizedDescription("fa-IR", CultureConstants.PANJ_SHANBE_FA)]
    [LocalizedDescription("en-US", CultureConstants.PANJ_SHANBE_EN)]
    PanjShanbeh,

    [LocalizedDescription("fa-IR", CultureConstants.JOME_FA)]
    [LocalizedDescription("en-US", CultureConstants.JOME_EN)]
    Jomeh,

    [LocalizedDescription("fa-IR", CultureConstants.SHANBE_FA)]
    [LocalizedDescription("en-US", CultureConstants.SHANBE_EN)]
    Shanbeh
}

public enum PersianMonth
{
    [LocalizedDescription("fa-IR", CultureConstants.FARVARDIN_FA)]
    [LocalizedDescription("en-US", CultureConstants.FARVARDIN_EN)]
    Farvardin,

    [LocalizedDescription("fa-IR", CultureConstants.ORDIBEHESHT_FA)]
    [LocalizedDescription("en-US", CultureConstants.ORDIBEHESHT_EN)]
    Ordibehesht,

    [LocalizedDescription("fa-IR", CultureConstants.KHORDAD_FA)]
    [LocalizedDescription("en-US", CultureConstants.KHORDAD_FA)]
    Khordad,

    [LocalizedDescription("fa-IR", CultureConstants.TIR_FA)]
    [LocalizedDescription("en-US", CultureConstants.TIR_EN)]
    Tir,

    [LocalizedDescription("fa-IR", CultureConstants.MORDAD_FA)]
    [LocalizedDescription("en-US", CultureConstants.MORDAD_EN)]
    Mordad,

    [LocalizedDescription("fa-IR", CultureConstants.SHAHRIVAR_FA)]
    [LocalizedDescription("en-US", CultureConstants.SHAHRIVAR_EN)]
    Sharivar,

    [LocalizedDescription("fa-IR", CultureConstants.MEHR_FA)]
    [LocalizedDescription("en-US", CultureConstants.MEHR_EN)]
    Mehr,

    [LocalizedDescription("fa-IR", CultureConstants.ABAN_FA)]
    [LocalizedDescription("en-US", CultureConstants.ABAN_EN)]
    Aban,

    [LocalizedDescription("fa-IR", CultureConstants.AZAR_FA)]
    [LocalizedDescription("en-US", CultureConstants.AZAR_EN)]
    Azar,

    [LocalizedDescription("fa-IR", CultureConstants.DEY_FA)]
    [LocalizedDescription("en-US", CultureConstants.DEY_EN)]
    Dey,

    [LocalizedDescription("fa-IR", CultureConstants.BAHMAN_FA)]
    [LocalizedDescription("en-US", CultureConstants.BAHMAN_EN)]
    Bahman,

    [LocalizedDescription("fa-IR", CultureConstants.ESFAND_FA)]
    [LocalizedDescription("en-US", CultureConstants.ESFAND_EN)]
    Esfand
}

public enum PercentPositivePattern
{
    /// <summary>
    /// n %
    /// </summary>
    [Description("n %")]
    Value0 = 0,

    /// <summary>
    /// n%
    /// </summary>
    [Description("n%")]
    Value1 = 1,

    /// <summary>
    /// %n
    /// </summary>
    [Description("%n")]
    Value2 = 2,

    /// <summary>
    /// % n
    /// </summary>
    [Description("% n")]
    Value3 = 3
}

public enum TimeBand
{
    Overnight,
    MorningRush,
    Daytime,
    Eveningrush
}