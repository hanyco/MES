using CodeGenerator.Application.Domain;
using Library.CodeGenLib.Back;
using Library.CodeGenLib.CodeGenerators;
using Library.CodeGenLib.Models;
using Library.Coding;

namespace CodeGenerator.Application.Services;

public partial class DtoService
{
    /// <summary>
    /// تولید کد کلاس DTO به صورت partial.
    /// </summary>
    public Code GenerateCode(Dto dto)
    {
        var ns = INamespace.New(dto.Namespace);
        var cls = IClass.New(dto.Name);
        foreach (var field in dto.Properties)
        {
            var prop = IProperty.New(field.Name, TypePath.New(field.TypeFullName ?? "object"));
            cls.AddProperty(prop);
        }
        ns.AddType(cls);

        var codeResult = ns.GenerateCode<RoslynCodeGenerator>();
        return new Code(dto.Name, Languages.CSharp, codeResult.Value!, isPartial: true);
    }
}
