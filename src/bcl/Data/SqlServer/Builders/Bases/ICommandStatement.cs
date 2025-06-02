namespace Library.Data.SqlServer.Builders.Bases;

public interface ICommandStatement
{
    bool ForceFormatValues { get; set; }
    bool ReturnId { get; set; }
}