using System;
using System.Collections.Generic;
using System.Text;

namespace FresherMisa2026.Entities.Employee
{
    public class FilterEmployeesRequest
    {
        /// <summary>
        /// Lọc theo phòng ban
        /// </summary>
        public Guid? DepartmentId { get; set; }

        /// <summary>
        /// Lọc theo vị trí
        /// </summary>
        public Guid? PositionId { get; set; }

        /// <summary>
        /// Lọc lương từ
        /// </summary>
        public decimal? SalaryFrom { get; set; }

        /// <summary>
        /// Lọc lương đến
        /// </summary>
        public decimal? SalaryTo { get; set; }

        /// <summary>
        /// 0: Nam, 1: Nữ, 2: Khác
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
        /// Số lượng bản ghi muốn lấy
        /// </summary>
        public int? PageSize { get; set; } = 10;
        /// <summary>
        /// Trang đang muốn lấy
        /// </summary>
        public int? PageIndex { get; set; } = 1;
    }
}
