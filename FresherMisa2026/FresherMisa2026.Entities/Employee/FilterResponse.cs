using System;
using System.Collections.Generic;
using System.Text;

namespace FresherMisa2026.Entities.Employee
{
    public class FilterResponse<TEntity>
    {
        public long Total { get; set; }

        public IEnumerable<TEntity> Data { get; set; }
    }
}
