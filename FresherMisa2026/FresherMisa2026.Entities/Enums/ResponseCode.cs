using System;
using System.Collections.Generic;
using System.Text;

namespace FresherMisa2026.Entities.Enums
{
    /// <summary>
    /// Mã trạng thái phản hồi API
    /// </summary>
    public enum ResponseCode
    {
        /// <summary>
        /// Thành công
        /// </summary>
        Success = 200,

        /// <summary>
        /// Tạo mới thành công
        /// </summary>
        Created = 201,

        /// <summary>
        /// Dữ liệu đầu vào không hợp lệ
        /// </summary>
        BadRequest = 400,

        /// <summary>
        /// Không tìm thấy dữ liệu
        /// </summary>
        NotFound = 404,

        /// <summary>
        /// Xung đột dữ liệu
        /// </summary>
        Conflict = 409,

        /// <summary>
        /// Lỗi hệ thống
        /// </summary>
        InternalServerError = 500
    }
}
