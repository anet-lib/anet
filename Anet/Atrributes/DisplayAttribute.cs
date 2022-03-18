﻿namespace Anet.Atrributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class DisplayAttribute : Attribute
{
    public DisplayAttribute()
    {
    }
    public DisplayAttribute(string label)
    {
        Label = label;
    }
    public string Label { get; set; }
    public int Order { get; set; }
    public bool Visible { get; set; } = true;
    public string Group { get; set; }
    public string Description { get; set; }
}
