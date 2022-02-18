namespace Anet.Models;

public class SelectOption : SelectOption<int>
{
}

public class SelectOption<TValue>
    where TValue : IEquatable<TValue>
{
    public TValue Value { get; set; }
    public string Label { get; set; }
    public int Order { get; set; }
    public string Group { get; set; }
    public bool Checked { get; set; }
}
