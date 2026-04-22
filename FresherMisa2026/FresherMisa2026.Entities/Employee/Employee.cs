using FresherMisa2026.Entities.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FresherMisa2026.Entities.Employee
{
    [ConfigTable("Employee", false, "EmployeeCode")]
    /// <summary>
    /// Thông tin nhân viên
    /// </summary>
    public class Employee : BaseModel
    {
        /// <summary>
        /// Id nhân viên
        /// </summary>
        [Key]
        public Guid EmployeeID { get; set; }

        /// <summary>
        /// Mã nhân viên
        /// </summary>
        [IRequired]
        public string? EmployeeCode { get; set; }

        /// <summary>
        /// Tên nhân viên
        /// </summary>
        [IRequired]
        public string? EmployeeName { get; set; }

        /// <summary>
        /// Giới tính
        /// </summary>
        public int Gender { get; set; }

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
        /// Mức lương
        /// </summary>
        public decimal Salary { get; set; }

        /// <summary>
        /// Id phòng ban
        /// </summary>
        [IRequired]
        public Guid? DepartmentID { get; set; }

        /// <summary>
        /// Id vị trí
        /// </summary>
        [IRequired]
        public Guid? PositionID { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Ngày nhận việc
        /// </summary>
        public DateTime HireDate { get; set; }
    }
}
