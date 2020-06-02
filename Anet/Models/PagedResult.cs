using System;
using System.Collections.Generic;

namespace Anet
{
    public class PagedResult<T> : IPagination
    {
        public PagedResult()
        {
        }

        public PagedResult(int page, int size)
        {
            Page = page;
            Size = size;
        }

        public int Page { get; set; }
        public int Size { get; set; }
        public int Total { get; set; }
        public IEnumerable<T> Items { get; set; } = new List<T>();

        public int TotalPages => (int)Math.Ceiling((decimal)Total / Size);
    }
}
