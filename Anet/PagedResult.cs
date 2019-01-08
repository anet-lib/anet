using System.Collections.Generic;

namespace Anet
{
    public class PagedResult<T>
    {
        public virtual int Total { get; set; }
        public virtual IEnumerable<T> List { get; set; }
    }
}
