using CodeGenerator.Designer.UI.ViewModels;

using DataLib;

namespace CodeGenerator.Application.Services;

public interface IDtoService : ICodeGenerator<Dto>, ICrudService<Dto>
{
    DtoViewModel CrateByTable(Table table);
}
