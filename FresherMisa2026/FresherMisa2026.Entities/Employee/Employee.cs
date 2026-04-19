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

        /// <summary>
        /// Mã nhân viên
        /// </summary>
        [IRequired]
        [Display(Name = "Mã nhân viên")]
        public string EmployeeCode { get; set; }

        /// <summary>
        /// Tên nhân viên
        /// </summary>
        [IRequired]
        [Display(Name = "Tên nhân viên")]
        public string EmployeeName { get; set; }

        public int? Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }

        /// <summary>
        /// Phòng ban
        /// </summary>
        [IRequired]
        [Display(Name = "Phòng ban")]
        public Guid DepartmentID { get; set; }

        /// <summary>
        /// Vị trí
        /// </summary>
        [IRequired]
        [Display(Name = "Vị trí")]
        public Guid PositionID { get; set; }

        public decimal? Salary { get; set; }

        /// <summary>
        /// Ngày vào làm từ
        /// </summary>
        public DateTime? HireDateFrom { get; set; }

        /// <summary>
        /// Ngày vào làm đến
        /// </summary>
        public DateTime? HireDateTo { get; set; }

        public DateTime? CreatedDate { get; set; }
    }
}