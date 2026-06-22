using System;

[AttributeUsage(AttributeTargets.Method)]
public class InspectorButtonAttribute : Attribute
{
    public string Label { get; }

    public InspectorButtonAttribute()
    {
    }

    public InspectorButtonAttribute(string label)
    {
        Label = label;
    }
}
