using FresherMisa2026.Entities.Extensions;
using System;
using System.ComponentModel.DataAnnotations;


namespace FresherMisa2026.Entities.Employee
{
    [ConfigTable("Employee", false, "EmployeeCode")]
    public class Employee : BaseModel
    {
        [Key]
        public Guid EmployeeID { get; set; }

        [IRequired]
        [Display(Name = "Mã nhân viên")]
        public string EmployeeCode { get; set; }

        [IRequired]
        [Display(Name = "Tên nhân viên")]
        public string EmployeeName { get; set; }

        [Display(Name = "Giới tính")]
        public int? Gender { get; set; }

        [Display(Name = "Ngày sinh")]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Số điện thoại")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "Địa chỉ")]
        public string? Address { get; set; }

        [IRequired]
        [Display(Name = "Phòng ban")]
        public Guid DepartmentID { get; set; }

        [IRequired]
        [Display(Name = "Chức vụ")]
        public Guid PositionID { get; set; }

        [Display(Name = "Lương")]
        public decimal? Salary { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime? CreatedDate { get; set; }
    }
}