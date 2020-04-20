using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Anet.Entity
{
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

    public class TextAttribute : ColumnAttribute
    {
        public TextAttribute()
        {
            TypeName = "text";
        }
    }

    public class VarcharAttribute : MaxLengthAttribute
    {
        public VarcharAttribute(int length = 255) : base(length)
        {
        }
    }
}
