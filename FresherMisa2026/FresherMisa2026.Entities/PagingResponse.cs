using System;
using System.Collections.Generic;
using System.Text;

namespace FresherMisa2026.Entities
{
    /// <summary>
    /// Dữ liệu phản hồi phân trang
    /// </summary>
    /// <typeparam name="T">Kiểu dữ liệu phần tử</typeparam>
    public class PagingResponse<T>
    {
        /// <summary>
        /// Tổng số bản ghi
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// Tổng số trang
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Trang hiện tại
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Số bản ghi mỗi trang
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Dữ liệu của trang hiện tại
        /// </summary>
        public List<T> Data { get; set; }
    }
}
