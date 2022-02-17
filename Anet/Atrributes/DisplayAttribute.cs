namespace Anet.Atrributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class DisplayAttribute : Attribute
{
    public DisplayAttribute(string name)
    {
        Name = name;
    }
    public string Name { get; set; }
    public int Order { get; set; }
    public string Group { get; set; }
    public string Description { get; set; }
}
