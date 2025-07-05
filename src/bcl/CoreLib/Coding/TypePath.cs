using System.Collections;
using System.Collections.Frozen;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Library.Coding;

[DebuggerStepThrough, StackTraceHidden]
public sealed class TypePath : IEquatable<TypePath>
{
    private static readonly Dictionary<Type, string> _primitiveTypes = new()
    {
        { typeof(int), "int" },
        { typeof(long), "long" },
        { typeof(byte), "byte" },
        { typeof(char), "char" },
        { typeof(bool), "bool" },
        { typeof(uint), "uint" },
        { typeof(nint), "nint" },
        { typeof(float), "float" },
        { typeof(nuint), "nuint" },
        { typeof(short), "short" },
        { typeof(sbyte), "sbyte" },
        { typeof(double), "double" },
        { typeof(string), "string" },
        { typeof(ushort), "ushort" },
        { typeof(decimal), "decimal" },
    };

    private readonly TypeData _data;
    private TypePath[]? _generics;

    public TypePath([DisallowNull] in string fullPath, in IEnumerable<string>? generics, bool? isNullable = null)
        : this(TypeData.Parse(fullPath, generics, isNullable))
    {
    }

    public TypePath([DisallowNull] in string fullPath)
        : this(TypeData.Parse(fullPath))
    {
    }

    internal TypePath(TypeData typeData)
        => this._data = typeData;

    [NotNull]
    public string FullName => field ??= this.GetFullName();

    [NotNull]
    public string FullPath => field ??= this.GetFullPath();

    [NotNull]
    public IEnumerable<TypePath> Generics => this._generics ??= [.. this._data.Generics.Select(x => new TypePath(x))];

    public bool IsGeneric => this.Generics.Any();

    public bool IsNullable => this._data.IsNullable;

    [NotNull]
    public string Name => this._data.Name;

    public string NameSpace => this._data.NameSpace ?? string.Empty;

    public static string AsKeyword(in string nameOrFullName)
    {
        var buffer = nameOrFullName;
        _primitiveTypes.ForEach(x => buffer = buffer.Replace($"System.{x.Key.Name}", x.Value).Replace($"{x.Key.Name}", x.Value));
        return buffer;
    }

    public static string Combine(in string? part1, params string?[] parts)
        => Enumerable.ToEnumerable(part1).AddRangeImmuted(parts).Compact().Select(x => x.Trim('.')).Merge('.');

    public static TypePath FromKeyword(in string keyword)
    {
        var isNullable = keyword.EndsWith('?');
        var kw = isNullable ? keyword.RemoveEnd("?") : keyword;
        var type = _primitiveTypes.FirstOrDefault(x => x.Value == kw).Key?.FullName ?? keyword;
        return Parse(type, isNullable);
    }

    [return: NotNullIfNotNull(nameof(typePath))]
    public static string? GetName(in string? typePath)
        => typePath == null ? null : TypeData.Parse(typePath).Name;

    [return: NotNullIfNotNull(nameof(typePath))]
    public static string? GetNameSpace(in string? typePath)
        => typePath == null ? null : TypeData.Parse(typePath).NameSpace ?? string.Empty;

    [return: NotNull]
    public static string GetNameSpace<T>()
    {
        var path = typeof(T).FullName;
        return path.IsNullOrEmpty() ? string.Empty : TypeData.Parse(path).NameSpace ?? string.Empty;
    }

    [return: NotNull]
    public static IEnumerable<string>? GetNameSpaces(in string? typePath)
        => typePath == null ? [] : Parse(typePath).GetNameSpaces();

    [return: NotNullIfNotNull(nameof(typeInfo))]
    public static implicit operator string?(in TypePath? typeInfo)
        => typeInfo?.FullName;

    [return: NotNullIfNotNull(nameof(typeInfo))]
    public static implicit operator TypePath?(in string? typeInfo)
        => typeInfo.IsNullOrEmpty() ? null : new(typeInfo);

    [return: NotNullIfNotNull(nameof(typeInfo))]
    public static implicit operator TypePath?(in Type? typeInfo)
        => typeInfo == null ? null : Parse(typeInfo);

    [return: NotNull]
    public static TypePath Parse([DisallowNull] in string name, in string? nameSpace)
        => new(Combine(nameSpace, name));

    [return: NotNull]
    public static TypePath Parse([DisallowNull] in string name, in string? nameSpace, in IEnumerable<string>? generics)
        => new(Combine(nameSpace, name), generics);

    [return: NotNull]
    public static TypePath Parse([DisallowNull] in string name, in string? nameSpace, in IEnumerable<string>? generics, bool isNullable)
        => new(Combine(nameSpace, name), generics, isNullable);

    [return: NotNull]
    public static TypePath Parse([DisallowNull] in string fullPath)
        => new(fullPath);

    [return: NotNull]
    public static TypePath Parse([DisallowNull] in string fullPath, in IEnumerable<string>? generics)
        => new(fullPath, generics);

    [return: NotNull]
    public static TypePath Parse([DisallowNull] in string fullPath, bool isNullable)
        => new(fullPath, null, isNullable);

    [return: NotNull]
    public static TypePath Parse([DisallowNull] in string fullPath, in IEnumerable<string>? generics, bool isNullable)
        => new(fullPath, generics, isNullable);

    [return: NotNull]
    public static TypePath Parse([DisallowNull] in Type type)
        => new((type?.FullName ?? null)!);

    [return: NotNull]
    public static TypePath Parse([DisallowNull] in Type type, in IEnumerable<Type>? generics)
        => new(type?.FullName!, generics?.Select(x => x.Name == "Nullable`1" ? $"{x.GenericTypeArguments[0].FullName}?" : x.FullName!));

    [return: NotNull]
    public static TypePath Parse([DisallowNull] in Type type, in IEnumerable<Type>? generics, bool isNullable)
        => new(type?.FullName!, generics?.Select(x => x.FullName!), isNullable);

    [return: NotNull]
    public static TypePath Parse<T>()
        => Parse(typeof(T));

    public static TypePath Parse<T>(in IEnumerable<string> generics)
        => Parse(typeof(T).FullName!, generics);

    [return: NotNull]
    public static TypePath Parse<T>(in IEnumerable<Type>? generics)
        => new(typeof(T).FullName!, generics?.Select(x => x.FullName!));

    [return: NotNull]
    public static TypePath Parse<T>(in IEnumerable<string>? generics, bool isNullable)
        => new(typeof(T).FullName!, generics, isNullable);

    [return: NotNull]
    public static TypePath ParseEnumerable(in TypePath generic)
        => Parse(typeof(IEnumerable<>).FullName!, [generic]);

    [return: NotNull]
    public static TypePath ParseEnumerable(in string generic)
        => Parse(typeof(IEnumerable<>).FullName!, [generic]);

    [return: NotNull]
    public static TypePath ParseEnumerable()
        => Parse<IEnumerable>();

    [return: NotNull]
    public static TypePath ParseTask()
        => Parse<Task>();

    public static TypePath ParseTask<TResult>()
        => Parse<Task>([typeof(TResult)]);

    [return: NotNull]
    public static TypePath ParseTask(in TypePath generic)
        => Parse(typeof(Task<>).FullName!, [generic.FullName]);

    [return: NotNull]
    public static TypePath ParseTask(in TypePath generic, bool isNullable)
        => Parse(typeof(Task<>).FullName!, [generic.FullName], isNullable);

    public static bool operator !=(in TypePath? left, in TypePath? right)
        => !(left == right);

    public static bool operator ==(in TypePath? left, in TypePath? right)
        => left?.Equals(right) ?? right is null;

    public string AsKeyword()
        => AsKeyword(this.FullName);

    public void Deconstruct(out string? name, out string? nameSpace)
        => (name, nameSpace) = (this.Name, this.NameSpace);

    public void Deconstruct(out string? name, out string? nameSpace, out IEnumerable<TypePath> generics)
        => (name, nameSpace, generics) = (this.Name, this.NameSpace, this.Generics);

    public bool Equals(TypePath? other)
        => other is not null && this.GetHashCode() == other.GetHashCode();

    public override bool Equals(object? obj)
        => obj is TypePath path && this.Equals(path);

    [return: NotNull]
    public IEnumerator<string> GetEnumerator()
    {
        yield return this.NameSpace;
        yield return this.Name;
    }

    public override int GetHashCode()
        => this.FullName.GetHashCode(StringComparison.Ordinal);

    [return: NotNull]
    public IEnumerable<string> GetNameSpaces()
    {
        return gather().Compact().Distinct();

        IEnumerable<string> gather()
        {
            if (!this.NameSpace.IsNullOrEmpty())
            {
                yield return this.NameSpace;
            }

            foreach (var generic in this.Generics)
            {
                foreach (var genericNamespace in generic.GetNameSpaces())
                {
                    yield return genericNamespace;
                }
            }
        }
    }

    [return: NotNull]
    public override string ToString()
        => this.FullName;

    public TypePath WithNullable(bool isNullable)
        => this.IsNullable == isNullable ? this : new(this.FullPath, [], isNullable);

    [return: NotNull]
    private string GetFullName()
    {
        var buffer = new StringBuilder(this.Name);
        if (this.Generics.Any())
        {
            _ = buffer.Append('<')
                .Append(this.Generics.Select(x => x.FullName).Merge(", "))
                .Append('>');
        }
        if (this.IsNullable)
        {
            _ = buffer.Append('?');
        }

        return buffer.ToString();
    }

    [return: NotNull]
    private string GetFullPath()
    {
        var buffer = new StringBuilder();
        if (!this.NameSpace.IsNullOrEmpty())
        {
            _ = buffer.Append(CultureInfo.CurrentCulture, $"{this.NameSpace}.");
        }
        _ = buffer.Append(this.Name);
        if (this.Generics.Any())
        {
            _ = buffer.Append('<')
                .Append(this.Generics.Select(x => x.GetFullPath())!.Merge(", "))
                .Append('>');
        }
        if (this.IsNullable)
        {
            _ = buffer.Append('?');
        }

        var result = buffer.ToString();
        return result;
    }

    internal readonly record struct TypeData(string Name, string? NameSpace, IEnumerable<TypeData> Generics, bool IsNullable)
    {
        private static TypeData InnerParse([DisallowNull] in string fullPath)
        {
            Check.MustBeArgumentNotNull(fullPath);

            TypeData result;

            if (!fullPath.Contains('<'))
            { // Not Generic
                result = processSingleType(fullPath);
            }
            else
            { // Generic
                result = processGenericType(fullPath);
            }

            return result;

            static TypeData processSingleType(string fullPath)
            {
                TypeData result;
                var isNullable = fullPath.EndsWith('?');
                var buffer = fullPath.TrimEnd('?');
                var parts = buffer.Split('.');
                var name = parts.Last().Remove("`1");
                result = parts.Length == 1
                    ? new TypeData(name, null, [], isNullable)
                    : new TypeData(name, parts.Take(parts.Length - 1).Merge('.'), [], isNullable);
                return result;
            }

            static TypeData processGenericType(string fullPath)
            {
                TypeData result;
                var isNullable = fullPath.EndsWith('?');
                var beginningOfGeneric = fullPath.IndexOf('<');
                var mainType = fullPath[..beginningOfGeneric];
                var genericTypes = fullPath[(beginningOfGeneric + 1)..fullPath.LastIndexOf('>')].Split(',');
                var mainTypeParseResult = InnerParse(mainType);
                var genericTypesParseResult = genericTypes.Select(x => InnerParse(x)).ToFrozenSet();
                result = mainTypeParseResult with { Generics = genericTypesParseResult, IsNullable = isNullable };
                return result;
            }
        }

        [DebuggerHidden]
        internal static TypeData Parse([DisallowNull] in string fullType, in IEnumerable<string>? generics = null, in bool? isNullable = null)
        {
            var mainType = fullType
                .ConvertClrFormToNormalForm()
                .ConvertClrNullableToNormalNullable();

            var genericTypes = generics?.Select(x => x
                .ConvertClrFormToNormalForm()
                .ConvertClrNullableToNormalNullable());

            Check.MustBeArgumentNotNull(mainType);
            if (hasGenerics(generics))
            {
                Check.MustBe(hasNoGenericInMainPath(mainType), () => $"Generic type already contains generic parameters");
            }

            Check.MustBe(genericTypes?.All(x => !x.IsNullOrEmpty()) ?? true, () => "Generic types cannot be null or empty.");

            var nullable = isNullable ?? mainType.EndsWith('?');
            var fullPath = genericTypes?.Any() ?? false
                ? $"{removeNullableSign(mainType)}<{mergeGenerics(genericTypes)}>{addNullableSign(nullable)}"
                : $"{removeNullableSign(mainType)}{addNullableSign(nullable)}";

            var result = InnerParse(fullPath);

            return result;

            static string removeNullableSign(string fullType) => fullType.RemoveEnd("?")!;

            static string mergeGenerics(IEnumerable<string> generics) => string.Join(',', generics);

            static string addNullableSign(bool isNullable) => isNullable ? "?" : "";

            static bool hasGenerics(IEnumerable<string>? generics)
                => generics?.Any() ?? false;

            static bool hasNoGenericInMainPath(string mainType)
                => !(mainType.Contains('<') && !mainType.Contains("<>"));
        }
    }
}

internal static class TypPathHelpers
{
    [return: NotNullIfNotNull(nameof(input))]
    public static string? ConvertClrFormToNormalForm(this string? input)
    {
        var result = input.ReformatGenerics();
        result = result.RemoveExtraData();
        result = result.RemoveEmptyGenerics();
        return result;
    }

    [return: NotNullIfNotNull(nameof(input))]
    public static string? ConvertClrNullableToNormalNullable(this string? input)
    {
        var clrPattern = @"System\.Nullable`\d\[\[(?<InnerType>.+?)\]\]";
        var normPattern = @"System\.Nullable\<(?<InnerType>.+?)\>";
        var result = input
            .ReplaceByRegEx(clrPattern, "${InnerType}?")
            .ReplaceByRegEx(normPattern, "${InnerType}?");

        return result;
    }

    [return: NotNullIfNotNull(nameof(input))]
    public static string? EvaluateByRegEx(in string? input, [DisallowNull] in string regex, in MatchEvaluator evaluator)
        => input is null ? null : Regex.Replace(input, regex, evaluator);

    [return: NotNullIfNotNull(nameof(input))]
    public static string? RemoveByRegEx(in string? input, [DisallowNull] string regex)
        => input is null ? null : Regex.Replace(input, regex, string.Empty);

    [return: NotNullIfNotNull(nameof(input))]
    public static string? ReplaceByRegEx(this string? input, [DisallowNull] in string regex, in string replacement)
        => input is null ? null : Regex.Replace(input, regex, replacement);

    [return: NotNullIfNotNull(nameof(input))]
    private static string? ReformatGenerics(this string? input)
    {
        var genericPattern = @"`\d\[\[(?<InnerType>(?>[^\[\]]+|(?<Depth>\[)|(?<-Depth>\]))*(?(Depth)(?!)))\]\]";
        var result = EvaluateByRegEx(input, genericPattern, match =>
        {
            var innerType = match.Groups["InnerType"].Value;
            var typeParams = innerType.Split("],[", StringSplitOptions.None);
            for (var i = 0; i < typeParams.Length; i++)
            {
                typeParams[i] = typeParams[i].ConvertClrFormToNormalForm();
            }
            return "<" + string.Join(", ", typeParams) + ">";
        });
        return result;
    }

    private static string? RemoveEmptyGenerics(this string? input)
        => RemoveByRegEx(input, @"`\d");

    private static string? RemoveExtraData(this string? input)
    {
        if (input == null)
        {
            return null;
        }

        var indexOfComma = input.IndexOf(',');
        if (indexOfComma < 0)
        {
            return input;
        }

        var indexOfEndGeneric = input.IndexOf('>');
        return indexOfEndGeneric >= 0
            ? input[indexOfComma..indexOfEndGeneric]
            : input[..indexOfComma];
    }
}