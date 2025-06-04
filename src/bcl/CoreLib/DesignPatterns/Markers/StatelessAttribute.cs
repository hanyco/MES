namespace Library.DesignPatterns.Markers;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public sealed class StatelessAttribute : Attribute
{
}