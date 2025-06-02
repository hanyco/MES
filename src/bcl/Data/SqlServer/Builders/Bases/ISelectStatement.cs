namespace Library.Data.SqlServer.Builders.Bases;

public interface ISelectStatement : IStatementOnTable, IWhereClause
{
    IList<string> Columns { get; }

    int? TopCount { get; set; }

    string? OrderByColumn { get; set; }

    OrderByDirection OrderByDirection { get; set; }

    bool WithNoLock { get; set; }

    string ToString() =>
        this.Build();
}