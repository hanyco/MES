using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using CodeGenerator.Application.Cqrs.Commands;
using CodeGenerator.Application.Domain;

using Dapper;

using MediatR;

namespace CodeGenerator.Application.Cqrs.Handlers;
public class GetAllDtosQueryHandler(IDbConnection db)
        : IRequestHandler<GetAllDtosQuery, IEnumerable<DtoDefinition>>
{
    private readonly IDbConnection _db = db;

    public async Task<IEnumerable<DtoDefinition>> Handle(GetAllDtosQuery _, CancellationToken ct)
    {
        const string sql = "SELECT * FROM DtoDefinitions";
        return await _db.QueryAsync<DtoDefinition>(sql);
    }
}