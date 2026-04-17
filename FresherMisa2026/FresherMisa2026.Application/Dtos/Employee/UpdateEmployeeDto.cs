namespace FresherMisa2026.Application.Dtos.Employee
{
    public class UpdateEmployeeDto
    {
        public string? EmployeeCode { get; set; }
        public string? EmployeeName { get; set; }
        public int? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public Guid? DepartmentID { get; set; }
        public Guid? PositionID { get; set; }
        public decimal? Salary { get; set; }
    }
}
