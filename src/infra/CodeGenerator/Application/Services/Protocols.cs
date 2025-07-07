using System.Diagnostics.CodeAnalysis;

using Library.CodeGenLib.Models;
using Library.Resulting;

namespace CodeGenerator.Application.Services;

public interface ICodeGenerator<TCodeGeneratorDto>
{
    [return: NotNull] IResult<Codes> GenerateCodes(TCodeGeneratorDto dto, CancellationToken ct = default);
}

public interface ICodeGeneratorDto;

public interface ICodeGeneratorService<TCodeGeneratorDto> : ICodeGenerator<TCodeGeneratorDto>, IService
    where TCodeGeneratorDto : ICodeGeneratorDto;

public interface ICrud<TCrudDto> : IView<TCrudDto>
{
    [return: NotNull] Task<IResult> Delete(long id, CancellationToken ct = default);

    [return: NotNull] Task<IResult<long>> Insert(TCrudDto dto, CancellationToken ct = default);

    [return: NotNull] Task<IResult> Update(long id, TCrudDto dto, CancellationToken ct = default);
}

public interface ICrudService<TCrudDto> : ICrud<TCrudDto>, IService;

public interface IService;

public interface IView<TViewDto>
{
    [return: NotNull] Task<IResult<IEnumerable<TViewDto>>> GetAll(CancellationToken ct = default);

    [return: NotNull] Task<IResult<TViewDto?>> GetById(long id, CancellationToken ct = default);
}

public interface IViewService<TViewDto> : IView<TViewDto>, IService;
