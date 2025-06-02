namespace Library.Data.Models;

public record struct DataColumnBindingInfo(string? Title, object? BindingPathOrElement, DataColumnBindingType DataType = DataColumnBindingType.Default) : IDataColumnBindingInfo
{
    string? IDataColumnBindingInfo.BindingPathOrElement { get; }

    public readonly void Deconstruct(out string? title, out object? bindingPathOrElement) =>
        (title, bindingPathOrElement) = (this.Title, this.BindingPathOrElement);

    public static implicit operator (string? Title, object? BindingPathOrElement)(DataColumnBindingInfo value)
        => (value.Title, value.BindingPathOrElement);
    public static implicit operator DataColumnBindingInfo((string? Title, object? BindingPathOeElement) value)
        => new(value.Title, value.BindingPathOeElement);
}

public enum DataColumnBindingType
{
    Default,
    Text,
    HtmlElement
}