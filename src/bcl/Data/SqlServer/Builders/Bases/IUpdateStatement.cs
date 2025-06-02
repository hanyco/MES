namespace Library.Data.SqlServer.Builders.Bases;

public interface IUpdateStatement : IStatementOnTable, IWhereClause, ICommandStatement
{
    IDictionary<string, object> Values { get; }
}
