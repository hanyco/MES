namespace Library.DesignPatterns.Markers;

/// <summary>All the methods finally return a meaningful instance</summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, Inherited = false)]
public sealed class FluentAttribute : Attribute
{
}
