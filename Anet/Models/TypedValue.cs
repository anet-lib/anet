namespace Anet.Models;

public class TypedValue<T>
{
    public TypedValue()
    {
    }

    public TypedValue(T value)
    {
        Value = value;
    }
    public T Value { get; set; }
}

public class TypedValue : TypedValue<string>
{
    public TypedValue()
    {
    }
    public TypedValue(string value) : base(value)
    {
    }
}

