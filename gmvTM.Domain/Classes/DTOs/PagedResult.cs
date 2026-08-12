using System;
using System.Collections.Generic;

namespace gmvTM.Domain
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public int TotalPages
        {
            get
            {
                if (this.PageSize <= 0)
                    return 0;

                return (int)Math.Ceiling(this.TotalCount / (double)this.PageSize);
            }
        }
    }
}
