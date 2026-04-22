using System;
using System.Collections.Generic;
using System.Text;

namespace FresherMisa2026.Entities.Exceptions
{
    /// <summary>
    /// Exception dùng cho các lỗi thao tác cơ sở dữ liệu
    /// </summary>
    public class DatabaseException : Exception
    {
        /// <summary>
        /// Mã lỗi database
        /// </summary>
        public string? ErrorCode { get; }

        /// <summary>
        /// Khởi tạo DatabaseException
        /// </summary>
        /// <param name="message">Thông báo lỗi</param>
        /// <param name="errorCode">Mã lỗi database</param>
        /// <param name="innerException">Lỗi gốc</param>
        public DatabaseException(string message, string? errorCode = null, Exception? innerException = null)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
        }
    }
}
