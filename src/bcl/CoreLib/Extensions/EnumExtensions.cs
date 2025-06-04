using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

using Library.Coding;
using Library.Extensions;
using Library.Globalization.Attributes;
using Library.Validations;

namespace Library.Helpers;

/// <summary>
/// A utility to do some common tasks about enumerations
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Adds a flag to the given enumeration.
    /// </summary>
    public static TEnum AddFlag<TEnum>(this TEnum enumeration, in TEnum item)
            where TEnum : Enum => (TEnum)Enum.ToObject(typeof(TEnum), Caster.Cast(enumeration).ToInt() | Caster.Cast(item).ToInt());
    /// <summary>
    /// Checks if the given enumeration contains the specified item.
    /// </summary>
    public static bool Contains<TEnum>(this Enum enumeration, TEnum item)
            where TEnum : Enum => Caster.Cast(item).ToInt() is 0
            ? Caster.Cast(enumeration).ToInt() == 0
            : (Caster.Cast(enumeration).ToInt() | Caster.Cast(item).ToInt()) == Caster.Cast(enumeration).ToInt();

    /// <summary>
    /// Converts an enum of type TSourceEnum to an enum of type TDestinationEnum.
    /// </summary>
    public static TDestinationEnum Convert<TSourceEnum, TDestinationEnum>(TSourceEnum enumValue)
            where TDestinationEnum : Enum => (TDestinationEnum)Enum.Parse(typeof(TDestinationEnum), enumValue?.ToString() ?? string.Empty);

    /// <summary>
    /// Converts the specified value to the specified enum type.
    /// </summary>
    /// <typeparam name="TEnum">The type of the enum.</typeparam>
    /// <param name="value">The value to convert.</param>
    /// <returns>The converted enum.</returns>
    public static TEnum Convert<TEnum>(object value)
            where TEnum : Enum => Convert<object, TEnum>(value);

    /// <summary>
    /// Gets the descriptions of the given enumerable items.
    /// </summary>
    /// <typeparam name="TEnum">The type of the enumerable items.</typeparam>
    /// <param name="items">The enumerable items.</param>
    /// <returns>The descriptions of the given enumerable items.</returns>
    public static IEnumerable<string?> GetDescriptions<TEnum>(IEnumerable<TEnum> items)
            where TEnum : Enum => items.Select(item => GetItemDescription(item));

    /// <summary>
    /// Gets the descriptions of the specified items in the given culture.
    /// </summary>
    /// <typeparam name="TEnum">The type of the items.</typeparam>
    /// <param name="items">The items.</param>
    /// <param name="cultureName">The culture name.</param>
    /// <returns>The descriptions of the specified items in the given culture.</returns>
    public static IEnumerable<string?> GetDescriptions<TEnum>(IEnumerable<TEnum> items, string cultureName)
            where TEnum : Enum => items.Select(item => GetItemDescription(item, cultureName: cultureName));

    /// <summary>
    /// </summary>
    /// <param name="value"></param>
    /// <typeparam name="TAttribute"></typeparam>
    /// <returns></returns>
    public static TAttribute? GetItemAttribute<TAttribute>(Enum? value)
        where TAttribute : Attribute => GetItemAttributes<TAttribute>(value)?.FirstOrDefault();

    /// <summary>
    /// Gets the attribute of the specified enum value.
    /// </summary>
    /// <typeparam name="TEnum">The type of the enum.</typeparam>
    /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
    /// <param name="value">The enum value.</param>
    /// <returns>The attribute of the specified enum value.</returns>
    public static object? GetItemAttribute<TEnum, TAttribute>([DisallowNull] TEnum value)
            where TAttribute : Attribute
            where TEnum : Enum
    {
        var attributes = (TAttribute[]?)value.ArgumentNotNull(nameof(value))
                                             .GetType()
                                             .GetField(value.ToString())?.GetCustomAttributes(typeof(TAttribute), false);
        return attributes?.Length > 0 ? attributes[0] : null;
    }

    /// <summary>
    /// Gets the item attributes of the specified value.
    /// </summary>
    /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>The item attributes of the specified value.</returns>
    public static IEnumerable<TAttribute>? GetItemAttributes<TAttribute>(Enum? value)
            where TAttribute : Attribute
    {
        if (value is null)
        {
            return [];
        }

        var attributes = (TAttribute[]?)value.GetType().GetField(value.ToString())?.GetCustomAttributes(typeof(TAttribute), false);
        return attributes?.Length > 0 ? attributes.AsEnumerable() : [];
    }

    /// <summary>
    /// Gets the description of the specified enum value.
    /// </summary>
    /// <param name="value">The enum value.</param>
    /// <param name="localized">Whether to get the localized description.</param>
    /// <param name="cultureName">The culture name.</param>
    /// <returns>The description of the specified enum value.</returns>
    public static string? GetItemDescription(Enum? value,
            bool localized = true,
            string cultureName = "en-US")
    {
        Check.MustBeArgumentNotNull(value);
        Check.MustBe(!string.IsNullOrEmpty(cultureName), () => new ArgumentException($"'{nameof(cultureName)}' cannot be null or empty", nameof(cultureName)));

        if (localized)
        {
            var descriptions = GetItemAttributes<LocalizedDescriptionAttribute>(value)?.ToArray();
            if (descriptions?.Any() is false)
            {
                return value.ToString().Separate() ?? string.Empty;
            }

            var description = descriptions?.FirstOrDefault(desc => cultureName.EqualsTo(desc.CultureName));
            return description is null ? value.ToString().Separate() : description.Description;
        }

        var descriptionAttribute = GetItemAttribute<DescriptionAttribute>(value);
        return descriptionAttribute is null
            ? value.ToString().Separate()
            : descriptionAttribute.Description;
    }

    /// <summary>
    /// Gets the items of the specified enum type.
    /// </summary>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    /// <returns>An <see cref="IEnumerableTEnum"/> of the enum items.</returns>
    public static IEnumerable<TEnum> GetItems<TEnum>() where TEnum : Enum
        => Enum.GetValues(typeof(TEnum)).Cast<TEnum>();

    /// <summary>
    /// Gets the values of a given enum type as a dictionary.
    /// </summary>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    /// <returns>A dictionary of the enum values and their names.</returns>
    public static IDictionary<TEnum, string>? GetValues<TEnum>()
        where TEnum : Enum
    {
        var type = typeof(TEnum);

        if (!type.IsEnum)
        {
            throw new ArgumentException("type must be an enumeration type.");
        }

        var pairs = from TEnum t in Enum.GetValues(type)
                    select new
                    {
                        Value = (TEnum)Enum.ToObject(type, t),
                        Text = Enum.GetName(type, t)
                    };

        return pairs?.ToDictionary(t => t.Value, t => t.Text);
    }

    /// <summary>
    /// Checks if the given Enum value is contained in the given range of Enum values.
    /// </summary>
    public static bool IsIn(this Enum value, params Enum[] range)
        => range.Contains(value);

    /// <summary>
    /// Checks if the given value is a member of the specified enum type.
    /// </summary>
    /// <typeparam name="TEnum">The enum type to check against.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <returns>True if the value is a member of the specified enum type, false otherwise.</returns>
    public static bool IsMemberOf<TEnum>(object value)
        where TEnum : Enum
        => int.TryParse(value.ToString(), out var iValue)
            ? Enum.IsDefined(typeof(TEnum), iValue)
            : Enum.IsDefined(typeof(TEnum), Parse(value));

    /// <summary>
    /// Merges all values of a given Enum into a single value.
    /// </summary>
    /// <typeparam name="TEnum">The type of the Enum.</typeparam>
    /// <returns>The merged Enum value.</returns>
    public static TEnum Merge<TEnum>()
        where TEnum : Enum
    {
        var result = Enum.GetValues(typeof(TEnum)).Cast<int>().Aggregate(0, (current, item) => current | item);
        return (TEnum)Enum.Parse(typeof(TEnum), Enum.GetName(typeof(TEnum), result)!);
    }

    /// <summary>
    /// Parses an IEnumerable of objects to an IEnumerable of Enums of type TEnum.
    /// </summary>
    /// <typeparam name="TEnum">The type of Enum to parse to.</typeparam>
    /// <param name="source">The IEnumerable of objects to parse.</param>
    /// <returns>An IEnumerable of Enums of type TEnum.</returns>
    public static IEnumerable<TEnum> ParseIf<TEnum>(IEnumerable source)
        where TEnum : Enum
    {
        if (source is null)
        {
            yield break;
        }

        foreach (var item in source)
        {
            if (item is not null && TryParse(item, out TEnum? result))
            {
                yield return result;
            }
        }
    }

    /// <summary>
    /// Removes a flag from an enumeration.
    /// </summary>
    public static TEnum RemoveFlag<TEnum>(TEnum enumeration, TEnum item)
        where TEnum : Enum => (TEnum)Enum.ToObject(typeof(TEnum), Caster.Cast(enumeration).ToInt() & ~Caster.Cast(item).ToInt());

    /// <summary>
    /// Converts a string to a generic Enum type.
    /// </summary>
    /// <typeparam name="TEnum">The type of the Enum.</typeparam>
    /// <param name="value">The string to convert.</param>
    /// <returns>The Enum value.</returns>
    public static TEnum ToEnum<TEnum>(string value)
        where TEnum : Enum => Caster.Cast(Enum.Parse(typeof(TEnum), value)).To<TEnum>();

    /// <summary>
    /// Converts an integer value to a generic Enum type.
    /// </summary>
    /// <typeparam name="TEnum">The type of the Enum.</typeparam>
    /// <param name="value">The integer value to convert.</param>
    /// <returns>The Enum value.</returns>
    public static TEnum ToEnum<TEnum>(int value)
        => Caster.Cast(Enum.Parse(typeof(TEnum), Enum.GetName(typeof(TEnum), value)!)).To<TEnum>();

    /// <summary>
    /// Tries to parse the specified value into an enum of type TEnum.
    /// </summary>
    /// <typeparam name="TEnum">The type of the enum.</typeparam>
    /// <param name="value">The value to parse.</param>
    /// <param name="result">The parsed enum value.</param>
    /// <returns>True if the value was successfully parsed, false otherwise.</returns>
    public static bool TryParse<TEnum>(object value, [NotNullWhen(true)] out TEnum? result)
        where TEnum : Enum
    {
        result = default;
        if (!IsMemberOf<TEnum>(value))
        {
            return false;
        }

        result = Convert<TEnum>(Parse(value));
        return true;
    }

    private static object Parse(object value) => value is string
        ? value.ToString()!.Contains('.')
            ? value.ToString()![(value.ToString()!.LastIndexOf(".") + 1)..]
            : value
        : value;
}