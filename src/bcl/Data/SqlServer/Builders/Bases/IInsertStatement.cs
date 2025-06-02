namespace Library.Data.SqlServer.Builders.Bases;

public interface IInsertStatement : IStatementOnTable, ICommandStatement
{
    IDictionary<string, object?> Values { get; }
}