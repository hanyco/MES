using Library.CodeGenLib.Models;
using Library.Resulting;

namespace CodeGenerator.Application.Services;

public interface ICodeGenerator<TCodeGeneratorDto>
{
    IResult<Codes> GenerateCodes(TCodeGeneratorDto dto, CancellationToken ct = default);
}

public interface ICodeGeneratorDto;

public interface ICodeGeneratorService<TCodeGeneratorDto> : ICodeGenerator<TCodeGeneratorDto>, IService
    where TCodeGeneratorDto : ICodeGeneratorDto;

public interface ICrud<TCrudDto> : IView<TCrudDto>
{
    Task<IResult> Delete(long id, CancellationToken ct = default);

    Task<IResult<long>> Insert(TCrudDto dto, CancellationToken ct = default);

    Task<IResult> Update(long id, TCrudDto dto, CancellationToken ct = default);
}

public interface ICrudService<TCrudDto> : ICrud<TCrudDto>, IService;

public interface IService;

public interface IView<TViewDto>
{
    /// <summary>
    /// Retrieves all items as a collection of view DTOs.
    /// </summary>
    /// <remarks>
    /// This method returns a task that represents the asynchronous operation. The result contains
    /// an <see cref="IResult{T}" /> wrapping an enumerable of view DTOs. If no items are found, the
    /// enumerable will be empty.
    /// </remarks>
    /// <param name="ct">
    /// A <see cref="CancellationToken" /> that can be used to cancel the operation. Defaults to
    /// <see langword="default" /> if not provided.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The result contains an <see
    /// cref="IResult{T}" /> wrapping an enumerable of <typeparamref name="TViewDto" /> objects. The
    /// enumerable will be empty if no items are available.
    /// </returns>
    Task<IResult<IEnumerable<TViewDto>>> GetAll(CancellationToken ct = default);

    Task<IResult<TViewDto?>> GetById(long id, CancellationToken ct = default);
}

public interface IViewService<TViewDto> : IView<TViewDto>, IService;