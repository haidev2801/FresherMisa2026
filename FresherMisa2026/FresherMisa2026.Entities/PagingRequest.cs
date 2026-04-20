using System;
using System.Collections.Generic;
using System.Text;

namespace FresherMisa2026.Entities
{
    public class PagingRequest
    {
        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public string Search { get; set; } = string.Empty;

        public string Sort { get; set; } = string.Empty; //vd: +ModifiedDate

        public string SearchFields { get; set; } = string.Empty;
    }

}
