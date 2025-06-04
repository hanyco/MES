namespace Library.Globalization;

public interface ILocalizer
{
    string ToString(in DateTime dateTime);
    string Translate(in string statement, in string? culture = null);
}
