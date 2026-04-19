using System;
using System.Collections.Generic;
using System.Text;

namespace FresherMisa2026.Entities.Employee
{
    public class EmployeeFilterRequest
    {
        public Guid? DepartmentId { get; set; }
        public Guid? PositionId { get; set; }
        public decimal? SalaryFrom { get; set; }
        public decimal? SalaryTo { get; set; }
        public int? Gender { get; set; }
        public DateTime? HireDateFrom { get; set; }
        public DateTime? HireDateTo { get; set; }
    }
}
