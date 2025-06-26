using System.ComponentModel;

using Library.Coding;
using Library.Globalization.Attributes;

namespace Library.Extensions;

/// <summary>
/// A utility to do some common tasks about enumerations
/// </summary>
public static class EnumExtensions
{
    extension(Enum)
    {
        /// <summary>
        /// Converts an enum of type TSourceEnum to an enum of type TDestinationEnum.
        /// </summary>
        public static TDestinationEnum Convert<TDestinationEnum>(object enumValue) =>
            (TDestinationEnum)Enum.Parse(typeof(TDestinationEnum), enumValue?.ToString() ?? string.Empty);

        /// <summary>
        /// Gets the items of the specified enum type.
        /// </summary>
        /// <typeparam name="TEnum"> The enum type. </typeparam>
        /// <returns> An <see cref="IEnumerableTEnum" /> of the enum items. </returns>
        public static IEnumerable<TEnum> GetItems<TEnum>() where TEnum : Enum =>
            Enum.GetValues(typeof(TEnum)).Cast<TEnum>();

        /// <summary>
        /// Checks if the given value is a member of the specified enum type.
        /// </summary>
        /// <typeparam name="TEnum"> The enum type to check against. </typeparam>
        /// <param name="value"> The value to check. </param>
        /// <returns> True if the value is a member of the specified enum type, false otherwise. </returns>
        public static bool IsMemberOf<TEnum>(object value)
            where TEnum : Enum =>
                int.TryParse(value.ToString(), out var iValue)
                    ? Enum.IsDefined(typeof(TEnum), iValue)
                    : Enum.IsDefined(typeof(TEnum), Parse(value));

        /// <summary>
        /// Converts a string to a generic Enum type.
        /// </summary>
        /// <typeparam name="TEnum"> The type of the Enum. </typeparam>
        /// <param name="value"> The string to convert. </param>
        /// <returns> The Enum value. </returns>
        public static TEnum ToEnum<TEnum>(string value)
            where TEnum : Enum =>
            Enum.Parse(typeof(TEnum), value).Cast().To<TEnum>();

        /// <summary>
        /// Converts an integer value to a generic Enum type.
        /// </summary>
        /// <typeparam name="TEnum"> The type of the Enum. </typeparam>
        /// <param name="value"> The integer value to convert. </param>
        /// <returns> The Enum value. </returns>
        public static TEnum ToEnum<TEnum>(int value) =>
            Enum.Parse(typeof(TEnum), Enum.GetName(typeof(TEnum), value)!).Cast().To<TEnum>();
    }

    extension(Enum @this)
    {
        /// <summary>
        /// Determines whether the current instance contains the specified enumeration value.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enumeration. Must be a type derived from <see cref="System.Enum"/>.</typeparam>
        /// <param name="item">The enumeration value to check for containment.</param>
        /// <returns><see langword="true"/> if the current instance contains the specified enumeration value;  otherwise, <see
        /// langword="false"/>.</returns>
        public bool Contains<TEnum>(TEnum item)
            where TEnum : Enum => item.Cast().ToInt() is 0
                ? @this.Cast().ToInt() == 0
                : (@this.Cast().ToInt() | item.Cast().ToInt()) == @this.Cast().ToInt();

        /// <summary>
        /// Determines whether the current enum value is included in the specified range of enum values.
        /// </summary>
        /// <remarks>This method checks if the current enum value exists within the provided range of enum
        /// values. The range can include multiple enum values passed as parameters.</remarks>
        /// <param name="range">A collection of enum values to check against. Must be of the same type as the current enum value.</param>
        /// <returns><see langword="true"/> if the current enum value is found in the specified range; otherwise, <see
        /// langword="false"/>.</returns>
        public bool IsIn(params Enum[] range) =>
            range.Contains(@this);
    }

    extension<TEnum>(TEnum @this)
        where TEnum : Enum
    {
        /// <summary>
        /// Adds a flag to the given enumeration.
        /// </summary>
        public TEnum AddFlag(in TEnum item) =>
            (TEnum)Enum.ToObject(typeof(TEnum), @this.Cast().ToInt() | item.Cast().ToInt());

        /// <summary>
        /// Removes a flag from an enumeration.
        /// </summary>
        public TEnum RemoveFlag(TEnum item) =>
            (TEnum)Enum.ToObject(typeof(TEnum), @this.Cast().ToInt() & ~item.Cast().ToInt());
    }

    extension(ReflectionHelper)
    {
        /// <summary>
        /// Gets the descriptions of the given enumerable items.
        /// </summary>
        /// <typeparam name="TEnum"> The type of the enumerable items. </typeparam>
        /// <param name="items"> The enumerable items. </param>
        /// <returns> The descriptions of the given enumerable items. </returns>
        public static IEnumerable<string?> GetDescriptions<TEnum>(IEnumerable<TEnum> items)
            where TEnum : Enum =>
            items.Select(item => GetItemDescription(item));

        /// <summary>
        /// Gets the descriptions of the specified items in the given culture.
        /// </summary>
        /// <typeparam name="TEnum"> The type of the items. </typeparam>
        /// <param name="items">       The items. </param>
        /// <param name="cultureName"> The culture name. </param>
        /// <returns> The descriptions of the specified items in the given culture. </returns>
        public static IEnumerable<string?> GetDescriptions<TEnum>(IEnumerable<TEnum> items, string cultureName)
            where TEnum : Enum => items.Select(item => GetItemDescription(item, cultureName: cultureName));

        /// <summary>
        /// Retrieves the first attribute of the specified type applied to the given enumeration value.
        /// </summary>
        /// <remarks>
        /// This method is useful for accessing metadata associated with enumeration values, such as
        /// custom attributes that provide additional information or behavior. If multiple
        /// attributes of the specified type are applied, only the first one is returned.
        /// </remarks>
        /// <typeparam name="TAttribute">
        /// The type of the attribute to retrieve. Must derive from <see cref="System.Attribute" />.
        /// </typeparam>
        /// <param name="value">
        /// The enumeration value whose attribute is to be retrieved. Can be <see langword="null" />.
        /// </param>
        /// <returns>
        /// The first attribute of type <typeparamref name="TAttribute" /> applied to the
        /// enumeration value, or <see langword="null" /> if no such attribute is found or if
        /// <paramref name="value" /> is <see langword="null" />.
        /// </returns>
        public static TAttribute? GetItemAttribute<TAttribute>(Enum? value)
            where TAttribute : Attribute => GetItemAttributes<TAttribute>(value)?.FirstOrDefault();

        /// <summary>
        /// Retrieves the custom attributes of the specified type applied to the given enumeration value.
        /// </summary>
        /// <remarks>
        /// This method uses reflection to retrieve attributes applied to the field corresponding to
        /// the provided enumeration value. If <paramref name="value" /> is <see langword="null" />,
        /// the method returns <see langword="null" />.
        /// </remarks>
        /// <typeparam name="TAttribute">
        /// The type of the attribute to retrieve. Must derive from <see cref="System.Attribute" />.
        /// </typeparam>
        /// <param name="value">
        /// The enumeration value for which to retrieve attributes. Can be <see langword="null" />.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}" /> containing the attributes of type <typeparamref
        /// name="TAttribute" /> applied to the specified enumeration value, or <see langword="null"
        /// /> if <paramref name="value" /> is <see langword="null" />. If no attributes of the
        /// specified type are found, an empty collection is returned.
        /// </returns>
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
        /// <param name="value">       The enum value. </param>
        /// <param name="localized">   Whether to get the localized description. </param>
        /// <param name="cultureName"> The culture name. </param>
        /// <returns> The description of the specified enum value. </returns>
        public static string? GetItemDescription(Enum? value,
                bool localized = true,
                string cultureName = "en-US")
        {
            Check.MustBeArgumentNotNull(value);
            Check.MustBe(!string.IsNullOrEmpty(cultureName), () => new ArgumentException($"'{nameof(cultureName)}' cannot be null or empty", nameof(cultureName)));

            if (localized)
            {
                var descriptions = GetItemAttributes<LocalizedDescriptionAttribute>(value)?.ToArray();
                if (descriptions!.IsEmpty)
                {
                    return value?.ToString().Separate();
                }

                var description = descriptions?.FirstOrDefault(desc => cultureName.EqualsTo(desc.CultureName));
                return description is null ? value?.ToString().Separate() : description.Description;
            }

            var descriptionAttribute = GetItemAttribute<DescriptionAttribute>(value);
            return descriptionAttribute is null
                ? value.ToString().Separate()
                : descriptionAttribute.Description;
        }
    }

    private static object Parse(object value) => value is string
        ? value.ToString()!.Contains('.')
            ? value.ToString()![(value.ToString()!.LastIndexOf('.') + 1)..]
            : value
        : value;
}