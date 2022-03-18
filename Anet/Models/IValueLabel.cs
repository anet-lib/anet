namespace Anet.Models;

public interface IValueLabel : IValueLabel<int>
{
}


public interface IValueLabel<TValue>
{
    public TValue Value { get; set; }
    public string Label { get; set; }
}
