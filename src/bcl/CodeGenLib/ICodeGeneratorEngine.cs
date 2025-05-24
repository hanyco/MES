namespace Library.CodeGenLib;

/// <summary>
/// Represents the engine that generates code.
/// </summary>
public interface ICodeGeneratorEngine
{
    /// <summary>
    /// Generates code from the namespace.
    /// </summary>
    /// <param name="nameSpace"></param>
    /// <returns></returns>
    IResult<string> Generate([DisallowNull] INamespace nameSpace);
}

public static class CodeGeneratorExtensions
{
    /// <summary>
    /// Generates code from the namespace.
    /// </summary>
    /// <param name="codeGenerator">Code generator engine.</param>
    /// <param name="nameSpace">Namespace to generate code from.</param>
    /// <param name="name">Name of the code.</param>
    /// <param name="language">Language of the code.</param>
    /// <param name="isPartial">Is the code partial.</param>
    /// <param name="fileName">The file name of the code.</param>
    /// <returns>Gives the result of the code generation.</returns>
    public static IResult<Code> Generate(
        this ICodeGeneratorEngine codeGenerator,
        in INamespace nameSpace,
        [DisallowNull] in string name,
        [DisallowNull] Language language,
        bool isPartial,
        string? fileName = null)
    {
        Check.MustBeArgumentNotNull(codeGenerator);
        var statement = codeGenerator.Generate(nameSpace);
        var code = new Code(name, language, RoslynHelper.ReformatCode(statement.GetValue()), isPartial, fileName);
        return Result.From<Code>(statement, code);
    }
}