using System;
using System.Collections.Generic;
using System.Text;

namespace FresherMisa2026.Entities
{
    /// <summary>
    /// Thông tin yêu cầu phân trang và tìm kiếm
    /// </summary>
    public class PagingRequest
    {
        /// <summary>
        /// Trang hiện tại
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// Số bản ghi trên một trang
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Từ khóa tìm kiếm
        /// </summary>
        public string Search { get; set; }

        /// <summary>
        /// Điều kiện sắp xếp, ví dụ: +ModifiedDate
        /// </summary>
        public string Sort { get; set; } //vd: +ModifiedDate

        /// <summary>
        /// DepartmentCode;DepartmentName
        /// </summary>
        public List<string> SearchFields { get; set; }
    }

}
