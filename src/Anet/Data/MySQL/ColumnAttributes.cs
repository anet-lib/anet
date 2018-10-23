using System.ComponentModel.DataAnnotations.Schema;

namespace Anet.Data.MySQL
{
    public class VarcharAttribute : ColumnAttribute
    {
        public VarcharAttribute(int lenth)
        {
            TypeName = $"varchar({lenth})";
        }
    }

    public class NVarcharAttribute : ColumnAttribute
    {
        public NVarcharAttribute(int lenth)
        {
            TypeName = $"nvarchar({lenth})";
        }
    }

    public class DecimalAttribute : ColumnAttribute
    {
        /// <param name="M">
        /// M is the maximum number of digits (the precision). It has a range of 1 to 65.
        /// </param>
        /// <param name="D">
        /// D is the number of digits to the right of the decimal point (the scale). 
        /// It has a range of 0 to 30 and must be no larger than M.
        /// </param>
        public DecimalAttribute(int M, int D)
        {
            TypeName = $"decimal({M},{D})";
        }
    }

    public class TextAttribute: ColumnAttribute
    {
        public TextAttribute()
        {
            TypeName = "text";
        }
    }
}
