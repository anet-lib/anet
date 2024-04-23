namespace Anet.Models;

public class SelectOption : SelectOption<long>
{
    public SelectOption() { }
    public SelectOption(long value, string label) : base(value, label) { }

    public new List<SelectOption> Children { get; set; }
}

public class SelectOption<TValue>
    where TValue : IEquatable<TValue>
{
    public SelectOption()
    {
    }
    public SelectOption(TValue value, string label)
    {
        Value = value;
        Label = label;
    }

    public TValue Value { get; set; }
    public string Label { get; set; }
    public string Name { get; set; }
    public int Order { get; set; }
    public string Group { get; set; }
    public bool Checked { get; set; }

    public List<SelectOption<TValue>> Children { get; set; }
}
