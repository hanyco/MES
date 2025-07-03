using System.Diagnostics.CodeAnalysis;

using Library.CodeGenLib.Models;
using Library.Resulting;

namespace CodeGenerator.Application.Services;

public interface ICodeGenerator<TCodeGeneratorDto>
    where TCodeGeneratorDto : IDto
{
    [return: NotNull] IResult<Codes> GenerateCodes(TCodeGeneratorDto dto);
}

public interface ICodeGeneratorDto : IDto;

public interface ICodeGeneratorService<TCodeGeneratorDto> : ICodeGenerator<TCodeGeneratorDto>, IService
    where TCodeGeneratorDto : ICodeGeneratorDto;

public interface ICrud<TCrudDto> : IView<TCrudDto>
    where TCrudDto : IDto
{
    [return: NotNull] IResult DeleteById(int id);

    [return: NotNull] IResult<int> Insert(TCrudDto dto);

    [return: NotNull] IResult Update(int id, TCrudDto dto);
}

public interface ICrudDto : IDto;

public interface ICrudService<TCrudDto> : ICrud<TCrudDto>, IService
    where TCrudDto : ICrudDto;

public interface IDto;

public interface IService;

public interface IView<TViewDto>
    where TViewDto : IDto
{
    [return: NotNull] IResult<IReadOnlyCollection<TViewDto>> GetAll();

    [return: NotNull] IResult<TViewDto> GetById(int id);
}

public interface IViewService<TViewDto> : IView<TViewDto>, IService
    where TViewDto : IDto;