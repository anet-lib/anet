using System.Collections.Generic;

namespace Anet.Data
{
    public class PagedResult<T>
    {
        public virtual int Page { get; set; }
        public virtual int Size { get; set; }
        public virtual int Total { get; set; }
        public virtual IEnumerable<T> List { get; set; }
    }
}
