namespace Library.Data.Models;

public interface IDataColumnBindingInfo
{
    string? BindingPathOrElement { get; }
    DataColumnBindingType DataType { get; }
    string? Title { get; }

    static IDataColumnBindingInfo New(string? title, string? bindingPath, DataColumnBindingType dataType = DataColumnBindingType.Default) =>
        new DataColumnBindingInfo(title, bindingPath, dataType);
}