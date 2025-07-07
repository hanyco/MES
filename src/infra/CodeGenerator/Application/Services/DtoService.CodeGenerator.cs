using System.Diagnostics.CodeAnalysis;

using CodeGenerator.Application.Domain;

using Library.CodeGenLib.Back;
using Library.CodeGenLib.CodeGenerators;
using Library.CodeGenLib.Extensions;
using Library.CodeGenLib.Models;
using Library.Coding;
using Library.Exceptions;
using Library.Resulting;
using Library.Validations;

namespace CodeGenerator.Application.Services;

public partial class DtoService : IDtoService
{
    [return: NotNull]
    public IResult<Codes> GenerateCodes(Dto dto, CancellationToken ct = default)
    {
        try
        {
            Check.MustBeNotNull(dto);
            Check.MustBe(!string.IsNullOrWhiteSpace(dto.Name) && !string.IsNullOrWhiteSpace(dto.Namespace), () => new ValidationException("invalid dto name or namespace"));

            var ns = INamespace.New(dto.Namespace);
            var cls = IClass.New(dto.Name);
            foreach (var field in dto.Properties)
            {
                var prop = IProperty.New(field.Name, TypePath.Parse(field.TypeFullName ?? "object"));
                _ = cls.AddProperty(prop);
            }
            _ = ns.AddType(cls);
            var codeResult = ns.GenerateCode<RoslynCodeGenerator>();
            if (codeResult?.IsSucceed is not true)
            {
                throw new Exception();
            }
            var code = new Code(dto.Name, Languages.CSharp, codeResult.Value!, isPartial: true);
            return Result.Success(code.ToCodes());
        }
        catch (Exception ex)
        {
            return Result.Fail<Codes>(ex);
        }
    }
}