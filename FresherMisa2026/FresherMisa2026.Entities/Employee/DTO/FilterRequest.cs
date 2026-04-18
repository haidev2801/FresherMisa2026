using System;
using System.Collections.Generic;
using System.Text;

namespace FresherMisa2026.Entities.Employee.DTO
{
    public class FilterRequest
    {
        public string? DepartmentId { get; set; }
        public string? PositionId { get; set; }
        public decimal? SalaryFrom { get; set; }
        public decimal? SalaryTo { get; set; }
        public int? Gender { get; set; }
        public DateTime? HireDateFrom { get; set; }
        public DateTime? HireDateTo { get; set; }
        public int? PageSize { get; set; }
        public int? PageIndex { get; set; }
    }
}
