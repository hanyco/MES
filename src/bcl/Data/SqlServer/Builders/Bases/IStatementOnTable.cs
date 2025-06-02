namespace Library.Data.SqlServer.Builders.Bases;

public interface IStatementOnTable : ISqlStatement
{
    string? Schema { get; set; }
    string TableName { get; set; }
}