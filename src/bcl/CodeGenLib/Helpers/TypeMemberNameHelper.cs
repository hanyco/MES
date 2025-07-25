namespace Library.CodeGenLib.Helpers;

[DebuggerStepThrough]
[StackTraceHidden]
public static class TypeMemberNameHelper
{
    public static string FixVariableName(in string memberName)
    {
        var illegalChars = new[] { "!", "#", "%", "^", "&", "*", "(", ")", "-", "+", "/", "\\", " " };
        var result = memberName.EnsureArgumentNotNull().Trim().ReplaceAll(illegalChars, "_");

        if (LanguageKeywords.Keywords.Contains(result))
        {
            result = $"@{result}";
        }

        return result;
    }

    public static string ToArgName(in string name)
    {
        var buffer = Initialize(name);
        var result = $"{buffer[Range.EndAt(1)].ToLower(System.Globalization.CultureInfo.CurrentCulture)}{buffer[1..]}";
        return FixVariableName(result);
    }

    public static string ToFieldName(in string name)
    {
        var buffer = Initialize(name);
        var result = $"_{buffer[Range.EndAt(1)].ToLower(System.Globalization.CultureInfo.CurrentCulture)}{buffer[1..]}";
        return FixVariableName(result);
    }

    public static string ToPropName(in string name)
    {
        var buffer = Initialize(name);
        var result = $"{buffer[Range.EndAt(1)].ToUpper(System.Globalization.CultureInfo.CurrentCulture)}{buffer[1..]}";
        return FixVariableName(result);
    }

    private static string Initialize(in string name)
    {
        var result = name.EnsureArgumentNotNull().Trim().TrimStart('_');
        // Support interfaces
        if (result.Length > 1 && result[0] is 'i' or 'I' && char.IsUpper(result[1]))
        {
            result = result[1..];
        }

        return result;
    }
}