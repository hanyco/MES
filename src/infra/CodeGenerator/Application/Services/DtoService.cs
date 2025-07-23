namespace CodeGenerator.Application.Services;

using System.Diagnostics.CodeAnalysis;
using System.Linq;

using CodeGenerator.Application.Domain;

using Library.CodeGenLib;
using Library.CodeGenLib.Back;
using Library.CodeGenLib.Models;
using Library.Resulting;
using Microsoft.Data.SqlClient;

internal sealed partial class DtoService(SqlConnection connection, ICodeGeneratorEngine<INamespace> codeGenerator) : IDtoService
{
    private readonly SqlConnection _connection = connection;
    private readonly ICodeGeneratorEngine<INamespace> _codeGenerator = codeGenerator;

    [return: NotNull]
    public IResult<Codes> GenerateCodes(Dto dto, CancellationToken ct = default)
    {
        if (dto is null)
        {
            return Result.Fail<Codes>(new ArgumentNullException(nameof(dto)));
        }

        var nameSpace = INamespace.New(dto.Namespace);
        var classType = IClass.New(dto.Name) with { InheritanceModifier = InheritanceModifier.Partial };

        if (!dto.BaseType.IsNullOrEmpty())
        {
            classType.AddBaseType(TypePath.Parse(dto.BaseType!));
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

            var setter = prop.HasSetter != false ? new PropertyAccessor() : null;
            var getter = prop.HasGetter != false ? new PropertyAccessor() : null;

            var property = new CodeGenProperty(prop.Name, type, setter: setter, getter: getter);
            classType.AddProperty(property);
        }

        nameSpace.AddType(classType);

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
    public Task<IResult> Delete(long id, CancellationToken ct = default) =>
        Task.FromResult<IResult>(Result.Fail(new NotImplementedException()));

    [return: NotNull]
    public Task<IResult<long>> Insert(Dto dto, CancellationToken ct = default) =>
        Task.FromResult<IResult<long>>(Result.Fail<long>(new NotImplementedException()));

    [return: NotNull]
    public Task<IResult> Update(long id, Dto dto, CancellationToken ct = default) =>
        Task.FromResult<IResult>(Result.Fail(new NotImplementedException()));
}
