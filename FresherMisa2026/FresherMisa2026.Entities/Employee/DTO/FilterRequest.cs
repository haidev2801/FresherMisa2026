using System;
using System.Collections.Generic;
using System.Text;

namespace FresherMisa2026.Entities.Employee.DTO
{
    /// <summary>
    /// Điều kiện lọc danh sách nhân viên
    /// </summary>
    public class FilterRequest
    {
        /// <summary>
        /// Id phòng ban
        /// </summary>
        public string? DepartmentId { get; set; }

        /// <summary>
        /// Id vị trí
        /// </summary>
        public string? PositionId { get; set; }

        /// <summary>
        /// Lương từ
        /// </summary>
        public decimal? SalaryFrom { get; set; }

        /// <summary>
        /// Lương đến
        /// </summary>
        public decimal? SalaryTo { get; set; }

        /// <summary>
        /// Giới tính
        /// </summary>
        public int? Gender { get; set; }

        /// <summary>
        /// Ngày vào làm từ
        /// </summary>
        public DateTime? HireDateFrom { get; set; }

        /// <summary>
        /// Ngày vào làm đến
        /// </summary>
        public DateTime? HireDateTo { get; set; }

        /// <summary>
        /// Kích thước trang
        /// </summary>
        public int? PageSize { get; set; }

        /// <summary>
        /// Chỉ số trang
        /// </summary>
        public int? PageIndex { get; set; }
    }
}
