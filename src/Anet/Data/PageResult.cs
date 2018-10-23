using System.Collections.Generic;

namespace Anet.Data
{
    public class PageResult<T> where T : class, new()
    {
        public int Total { get; set; }
        public IEnumerable<T> List { get; set; }
    }
}
