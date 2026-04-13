using System;
using System.Collections.Generic;
using System.Text;

namespace FresherMisa2026.Entities
{
    /// <summary>
    /// Lớp chuẩn để các service/controller trả về kết quả.
    /// Thống nhất cấu trúc response giữa các layer: IsSuccess, Code (http-like), Data, UserMessage, DevMessage.
    /// </summary>
    public class ServiceResponse
    {
        /// <summary>
        /// True nếu thao tác thành công, false nếu có lỗi.
        /// </summary>
        public bool IsSuccess { get; set; }
        
        /// <summary>
        /// Mã trả về (tương tự mã HTTP như 200, 201, 400, 500...).
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Dữ liệu trả về (có thể là object, danh sách, số bản ghi bị ảnh hưởng...)
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// Thông điệp dành cho người dùng (có thể là message đã được chuyển ngôn ngữ).
        /// </summary>
        public object UserMessage { get; set; }

        /// <summary>
        /// Thông điệp dành cho developer (chi tiết lỗi, stack trace, thông tin debug).
        /// </summary>
        public object DevMessage { get; set; }
    }
}
