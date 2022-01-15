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

public class ValueModel : TypedValue<string>
{
    public ValueModel()
    {
    }
    public ValueModel(string value) : base(value)
    {
    }
}

