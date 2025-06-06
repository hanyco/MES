namespace Library.CodeGenLib;

/// <summary>
/// Defines the contract for a code generation engine that produces results based on a provided data
/// transfer object (DTO).
/// </summary>
/// <typeparam name="TDto">
/// The type of the data transfer object used as input for the code generation process.
/// </typeparam>
public interface ICodeGeneratorEngine<in TDto>
{
    /// <summary>
    /// Generates a result based on the provided data transfer object (DTO).
    /// </summary>
    /// <param name="dto"> The data transfer object used to generate the result. Cannot be null. </param>
    /// <returns>
    /// An <see cref="IResult{T}" /> containing the generated string result. The result may indicate
    /// success or failure.
    /// </returns>
    IResult<string> Generate([DisallowNull] TDto dto);
}

/// <summary>
/// Defines a contract for a code generation engine that produces a result based on input data and
/// configuration options.
/// </summary>
/// <typeparam name="TDto">
/// The type of the data transfer object (DTO) containing the input data required for generation.
/// </typeparam>
/// <typeparam name="TOptions"> The type of the options used to configure the generation process. </typeparam>
public interface ICodeGeneratorEngine<in TDto, in TOptions>
{
    /// <summary>
    /// Generates a result based on the provided data transfer object (DTO) and options.
    /// </summary>
    /// <param name="dto">    
    /// The data transfer object containing the input data required for generation. Cannot be null.
    /// </param>
    /// <param name="options"> The options that configure the generation process. </param>
    /// <returns>
    /// An <see cref="IResult{T}" /> containing the generated string result. The result may indicate
    /// success or failure.
    /// </returns>
    IResult<string> Generate([DisallowNull] TDto dto, TOptions options);
}

public static class CodeGeneratorExtensions
{
    /// <summary>
    /// Generates code from the namespace.
    /// </summary>
    /// <param name="codeGenerator"> Code generator engine. </param>
    /// <param name="nameSpace">     Namespace to generate code from. </param>
    /// <param name="name">          Name of the code. </param>
    /// <param name="language">      Language of the code. </param>
    /// <param name="isPartial">     Is the code partial. </param>
    /// <param name="fileName">      The file name of the code. </param>
    /// <returns> Gives the result of the code generation. </returns>
    public static IResult<Code> Generate(
        this ICodeGeneratorEngine<INamespace> codeGenerator,
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