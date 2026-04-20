using System;

namespace FresherMisa2026.Entities.Employee
{
    public class EmployeeFilterRequest
    {
        public Guid? DepartmentID { get; set; }
        public Guid? PositionID { get; set; }
        public decimal? SalaryFrom { get; set; }
        public decimal? SalaryTo { get; set; }
        public int? Gender { get; set; }
        public DateTime? HireDateFrom { get; set; }
        public DateTime? HireDateTo { get; set; }

        /// <summary>Trang bắt đầu từ 1.</summary>
        public int PageIndex { get; set; } = 1;

        /// <summary>Số bản ghi mỗi trang (mặc định 10).</summary>
        public int PageSize { get; set; } = 10;
    }
}