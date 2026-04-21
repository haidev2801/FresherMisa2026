using System;
using System.Collections.Generic;
using System.Text;

namespace FresherMisa2026.Entities
{
    public class FilterResponse<TEntity>
    {
        public long Total { get; set; }
        public long PageSize { get; set; } = 0;
        public long PageIndex { get; set; } = 1;
        public IEnumerable<TEntity> Data { get; set; }
    }
}
