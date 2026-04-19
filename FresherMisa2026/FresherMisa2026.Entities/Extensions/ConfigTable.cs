using System;
using System.Collections.Generic;
using System.Text;

namespace FresherMisa2026.Entities.Extensions
{
    /// <summary>
    /// Attribute dùng để cấu hình thông tin mapping giữa lớp entity và bảng trong database.
    /// Gắn lên class entity (ví dụ: [ConfigTable("Department", false, "DepartmentCode")])
    /// để chỉ định:
    /// - TableName: tên bảng tương ứng trong DB
    /// - HasDeletedColumn: bảng có dùng cột IsDeleted (xóa mềm) hay không
    /// - UniqueColumns: danh sách cột unique (dùng cho validate/kiểm tra trùng dữ liệu)
    /// </summary>
    public class ConfigTable : Attribute
    {
        /// <summary>
        /// Nếu true thì lớp này ánh xạ tới table có cột IsDeleted (xóa mềm).
        /// Repository sẽ tự thêm điều kiện IsDeleted = FALSE khi truy vấn nếu giá trị là true.
        /// Mặc định: false.
        /// </summary>
        public bool HasDeletedColumn { get; set; } = false;

        /// <summary>
        /// Tên các cột cần đảm bảo unique (tách nhau bằng dấu phẩy nếu nhiều).
        /// Dùng để service/repository kiểm tra trùng dữ liệu trước khi insert/update.
        /// Mặc định: chuỗi rỗng.
        /// </summary>
        public string UniqueColumns { get; set; } = string.Empty;

        /// <summary>
        /// Tên bảng trong database tương ứng với entity.
        /// Ví dụ: "Department".
        /// Nếu không cung cấp, các helper reflection có thể ném lỗi.
        /// </summary>
        public string TableName { get; set; } = string.Empty;

        /// <summary>
        /// Constructor của attribute ConfigTable.
        /// Thường gọi khi gắn attribute lên class entity, ví dụ:
        /// [ConfigTable("Department", false, "DepartmentCode")]
        /// Param:
        /// - tableName: tên bảng (chuỗi) tương ứng
        /// - hasDeletedColumn: true nếu bảng có cột IsDeleted
        /// - uniqueColumns: danh sách cột unique dùng để kiểm tra trùng
        /// </summary>
        /// <param name="tableName">Tên bảng (mặc định chuỗi rỗng)</param>
        /// <param name="hasDeletedColumn">Bảng có cột IsDeleted hay không (mặc định false)</param>
        /// <param name="uniqueColumns">Danh sách cột unique (mặc định chuỗi rỗng)</param>
        public ConfigTable(string tableName = "", bool hasDeletedColumn = false, string uniqueColumns = "")
        {
            // Gán tên bảng
            TableName = tableName;

            // Gán flag có cột IsDeleted hay không
            HasDeletedColumn = hasDeletedColumn;

            // Gán chuỗi các cột unique
            UniqueColumns = uniqueColumns;
        }
    }
}
