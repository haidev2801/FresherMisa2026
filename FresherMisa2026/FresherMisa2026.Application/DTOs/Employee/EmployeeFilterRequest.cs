namespace FresherMisa2026.Application.DTOs.Employee
{
    public class EmployeeFilterRequest
    {
        public Guid? DepartmentId { get; set; }

        public Guid? PositionId { get; set; }

        public decimal? SalaryFrom { get; set; }

        public decimal? SalaryTo { get; set; }

        /// <summary>
        /// 0: Nam, 1: Nu, 2: Khac
        /// </summary>
        public int? Gender { get; set; }

        public DateTime? HireDateFrom { get; set; }

        public DateTime? HireDateTo { get; set; }

        public int PageSize { get; set; } = 2;

        public int PageIndex { get; set; } = 1;

        /// <summary>
        /// salary_asc | salary_desc | hire_asc | hire_desc
        /// </summary>
        public string? OrderBy { get; set; }
    }
}