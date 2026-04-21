using System;
using System.Collections.Generic;
using System.Text;

namespace FresherMisa2026.Entities
{
    /// <summary>
    /// Dữ liệu phản hồi chuẩn từ tầng service
    /// </summary>
    public class ServiceResponse
    {
        /// <summary>
        /// Trạng thái xử lý thành công hay không
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Mã trạng thái phản hồi
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Dữ liệu trả về
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// Thông báo hiển thị cho người dùng
        /// </summary>
        public object UserMessage { get; set; }

        /// <summary>
        /// Thông báo chi tiết cho lập trình viên
        /// </summary>
        public object DevMessage { get; set; }
    }
}
