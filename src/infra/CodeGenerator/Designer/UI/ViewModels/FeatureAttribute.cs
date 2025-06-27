using System;

namespace CodeGenerator.Designer.UI.ViewModels;

[AttributeUsage(AttributeTargets.Class)]
public sealed class FeatureAttribute(string title) : Attribute
{
    public string Title { get; } = title;
}
