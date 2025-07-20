namespace CodeGenerator.Application.Services;

using System.Diagnostics.CodeAnalysis;

using CodeGenerator.Application.Domain;

using Library.CodeGenLib.Models;
using Library.Resulting;
using Microsoft.Data.SqlClient;

internal sealed partial class DtoService(SqlConnection connection) : IDtoService
{
    private readonly SqlConnection _connection = connection;

    [return: NotNull]
    public IResult<Codes> GenerateCodes(Dto dto, CancellationToken ct = default) =>
        Result.Fail<Codes>(new NotImplementedException());

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
