using System;

namespace FresherMisa2026.Entities.Employee.DTO
{
    public class EmployeeRaceConditionTestRequest
    {
        public string EmployeeCode { get; set; } = string.Empty;

        public string EmployeeName { get; set; } = "Nhan vien test race";

        public Guid DepartmentID { get; set; }

        public Guid PositionID { get; set; }

        public int? Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }

        public decimal? Salary { get; set; }

        public DateTime? HireDate { get; set; }
    }
}
