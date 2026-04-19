using FresherMisa2026.Entities.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FresherMisa2026.Entities.Employee
{
    [ConfigTable("Employee", false, "EmployeeCode")]
    public class Employee : BaseModel
    {
        /// <summary>
        /// ID nhân viên
        /// </summary>
        [Key]
        public Guid EmployeeID { get; set; }

        /// <summary>
        /// Mã nhân viên
        /// </summary>
        [IRequired]
        public string EmployeeCode { get; set; }

        /// <summary>
        /// Tên nhân viên
        /// </summary>
        [IRequired]
        public string EmployeeName { get; set; }

        /// <summary>
        /// Giới tính (0: Nữ, 1: Nam, 2: Khác)
        /// </summary>
        public int? Gender { get; set; }

        /// <summary>
        /// Ngày sinh
        /// </summary>
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// Số điện thoại
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Địa chỉ
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// ID phòng ban
        /// </summary>
        [IRequired]
        public Guid DepartmentID { get; set; }

        /// <summary>
        /// ID chức vụ
        /// </summary>
        [IRequired]
        public Guid PositionID { get; set; }

        /// <summary>
        /// Mức lương
        /// </summary>
        public decimal? Salary { get; set; }

        /// <summary>
        /// Ngày vào làm
        /// </summary>
        public DateTime? HireDate { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime? CreatedDate { get; set; }
    }
}