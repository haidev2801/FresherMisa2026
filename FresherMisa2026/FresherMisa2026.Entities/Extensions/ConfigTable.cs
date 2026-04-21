using System;
using System.Collections.Generic;
using System.Text;

namespace FresherMisa2026.Entities.Extensions
{
    /// <summary>
    /// Cấu hình ánh xạ thực thể với bảng dữ liệu
    /// </summary>
    public class ConfigTable : Attribute
    {
        /// <summary>
        /// Đánh dấu bảng có cột xóa mềm
        /// </summary>
        public bool HasDeletedColumn { get; set; } = false;

        /// <summary>
        /// Danh sách cột unique
        /// </summary>
        public string UniqueColumns { get; set; } = string.Empty;

        /// <summary>
        /// Tên bảng trong cơ sở dữ liệu
        /// </summary>
        public string TableName { get; set; } = string.Empty;

        /// <summary>
        /// Khởi tạo cấu hình bảng
        /// </summary>
        /// <param name="tableName">Tên bảng</param>
        /// <param name="hasDeletedColumn">Có cột xóa mềm hay không</param>
        /// <param name="uniqueColumns">Danh sách cột unique</param>
        public ConfigTable(string tableName = "", bool hasDeletedColumn = false, string uniqueColumns = "")
        {
            TableName = tableName;

            HasDeletedColumn = hasDeletedColumn;

            UniqueColumns = uniqueColumns;
        }
    }
}
