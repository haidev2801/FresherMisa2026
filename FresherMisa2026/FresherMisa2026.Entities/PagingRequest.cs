namespace FresherMisa2026.Entities
{
    public class PagingRequest
    {
        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public string? Search { get; set; }

        public string? Sort { get; set; } //vd: +ModifiedDate

        /// <summary>
        /// DepartmentCode;DepartmentName
        /// </summary>
        public string? SearchFields { get; set; }
    }

}
