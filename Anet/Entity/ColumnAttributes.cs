using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Anet.Entity;

public class DecimalAttribute : ColumnAttribute
{
    /// <param name="M">
    /// M is the maximum number of digits (the precision). It has a range of 1 to 65.
    /// </param>
    /// <param name="D">
    /// D is the number of digits to the right of the decimal point (the scale). 
    /// It has a range of 0 to 30 and must be no larger than M.
    /// </param>
    public DecimalAttribute(ushort M, ushort D)
    {
        TypeName = $"decimal({M},{D})";
    }
}

public class VarcharAttribute : MaxLengthAttribute
{
    public VarcharAttribute(int length = 255) : base(length)
    {
    }
}

public class CharAttribute : ColumnAttribute
{
    public CharAttribute(ushort length)
    {
        TypeName = $"char({length})";
    }
}

public class TextAttribute : ColumnAttribute
{
    public TextAttribute()
    {
        TypeName = "text";
    }
}

public class DateAttribute : ColumnAttribute
{
    public DateAttribute()
    {
        TypeName = "date";
    }
}

public class DatetimeAttribute : ColumnAttribute
{
    public DatetimeAttribute(ushort precision = 0)
    {
        TypeName = $"datetime({precision})";
    }
}
