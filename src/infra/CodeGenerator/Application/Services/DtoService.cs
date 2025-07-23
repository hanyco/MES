using System.Diagnostics.CodeAnalysis;

using Library.CodeGenLib;
using Library.CodeGenLib.Back;
using Library.CodeGenLib.Models;
using Library.Coding;
using Library.Resulting;

using Microsoft.Data.SqlClient;

namespace CodeGenerator.Application.Services;

internal sealed partial class DtoService(SqlConnection connection, ICodeGeneratorEngine<INamespace> codeGenerator) : IDtoService
{
    private readonly ICodeGeneratorEngine<INamespace> _codeGenerator = codeGenerator;
    private readonly SqlConnection _connection = connection;

    [return: NotNull]
    public Task<IResult> Delete(long id, CancellationToken ct = default) =>
        Task.FromResult<IResult>(Result.Fail(new NotImplementedException()));

    [return: NotNull]
    public IResult<Codes> GenerateCodes(Dto dto, CancellationToken ct = default)
    {
        if (dto is null)
        {
            return Result.Fail<Codes>(new ArgumentNullException(nameof(dto)));
        }

        var nameSpace = INamespace.New(dto.Namespace);
        var classType = new Class(dto.Name) { InheritanceModifier = InheritanceModifier.Partial };

        if (!dto.BaseType.IsNullOrEmpty())
        {
            _ = classType.AddBaseType(TypePath.Parse(dto.BaseType!));
        }

        foreach (var prop in dto.Properties)
        {
            var type = TypePath.Parse(prop.TypeFullName ?? "object");
            if (prop.IsList == true)
            {
                type = TypePath.ParseEnumerable(type);
            }

            if (prop.IsNullable == true)
            {
                type = type.WithNullable(true);
            }

            PropertyAccessor? setter = prop.HasSetter is not true ? null : new PropertyAccessor();
            PropertyAccessor? getter = prop.HasGetter is not true ? null : new PropertyAccessor();

            var property = new CodeGenProperty(prop.Name, type, setter: setter, getter: getter);
            _ = classType.AddProperty(property);
        }

        _ = nameSpace.AddType(classType);

        var codeResult = this._codeGenerator.Generate(nameSpace, dto.Name, Languages.CSharp, true);
        return Result.From<Codes>(codeResult, new Codes(codeResult.Value));
    }

    [return: NotNull]
    public Task<IResult<IEnumerable<Dto>>> GetAll(CancellationToken ct = default) =>
        Task.FromResult<IResult<IEnumerable<Dto>>>(Result.Fail<IEnumerable<Dto>>(new NotImplementedException()));

    [return: NotNull]
    public Task<IResult<Dto?>> GetById(long id, CancellationToken ct = default) =>
        Task.FromResult<IResult<Dto?>>(Result.Fail<Dto?>(new NotImplementedException()));

    [return: NotNull]
    public Task<IResult<long>> Insert(Dto dto, CancellationToken ct = default) =>
        Task.FromResult<IResult<long>>(Result.Fail<long>(new NotImplementedException()));

    [return: NotNull]
    public Task<IResult> Update(long id, Dto dto, CancellationToken ct = default) =>
        Task.FromResult<IResult>(Result.Fail(new NotImplementedException()));
}