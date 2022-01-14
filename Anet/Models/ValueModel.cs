namespace Anet.Web;

public class ValueModel<T>
{
    public ValueModel()
    {
    }

    public ValueModel(T value)
    {
        Value = value;
    }
    public T Value { get; set; }
}

public class ValueModel : ValueModel<string>
{
    public ValueModel()
    {
    }
    public ValueModel(string value) : base(value)
    {
    }
}

