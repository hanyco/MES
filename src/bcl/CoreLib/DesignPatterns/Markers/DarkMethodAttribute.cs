namespace Library.DesignPatterns.Markers;

[AttributeUsage(AttributeTargets.Method)]
public sealed class DarkMethodAttribute : Attribute
{
    public string? Reason { get; set; }
}