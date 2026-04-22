using System;
using System.Collections.Generic;
using System.Text;

namespace FresherMisa2026.Entities
{
    /// <summary>
    /// Trạng thái thao tác dữ liệu của thực thể
    /// </summary>
    public enum ModelSate
    {
        /// <summary>
        /// Trạng thái thêm mới
        /// </summary>
        Add = 1,

        /// <summary>
        /// Trạng thái cập nhật
        /// </summary>
        Update = 2,

        /// <summary>
        /// Trạng thái xóa
        /// </summary>
        Delete = 3
    }
}
