using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FresherMisa2026.Entities.Extensions
{

    /// <summary>
    /// Các phương thức extension dùng reflection để lấy thông tin cấu hình giữa entity và database.
    /// Helper này đọc Attribute ConfigTable, DisplayAttribute, KeyAttribute... để cung cấp tên bảng, tên khoá chính, tên hiển thị...
    /// Các phương thức được dùng ở repository/service để tự động hoá việc mapping và validate.
    /// </summary>
    public static class MethodExtensions
    {
        /// <summary>
        /// Lấy tên bảng (TableName) từ attribute ConfigTable gắn trên class.
        /// Nếu không có attribute sẽ ném lỗi.
        /// </summary>
        /// <returns>Tên bảng trong DB</returns>
        public static string GetTableName(this Type type)
        {
            var configTable = GetConfigTable(type);
            if (configTable == null)
            {
                if (string.IsNullOrWhiteSpace(type.Name))
                {
                    throw new ArgumentException($"{nameof(type)} không có tên table");
                }
            }
            ;
            return configTable.TableName;
        }

        /// <summary>
        /// Lấy chuỗi các cột unique (nếu có) từ attribute ConfigTable.
        /// Dùng trong logic kiểm tra trùng dữ liệu trước khi insert/update.
        /// </summary>
        public static string GetUniqueColumns(this Type type)
        {
            var configTable = GetConfigTable(type);
            return configTable.UniqueColumns;
        }

        /// <summary>
        /// Lấy tên hiển thị của một property dựa vào DisplayAttribute.
        /// Nếu không có DisplayAttribute trả về chính tên property.
        /// </summary>
        /// <param name="name">Tên property</param>
        public static string GetColumnDisplayName(this Type type, string name)
        {
            var obj = type.GetProperty(name).GetCustomAttributes(typeof(DisplayAttribute),
                                               false).Cast<DisplayAttribute>().SingleOrDefault();
            if (obj == null) return name;

            return obj.Name;
        }

        /// <summary>
        /// Kiểm tra cấu hình table có cột IsDeleted (xóa mềm) hay không.
        /// Nếu true thì repository sẽ thêm điều kiện lọc IsDeleted = FALSE trong truy vấn đọc.
        /// </summary>
        public static bool GetHasDeletedColumn(this Type type)
        {
            var configTable = GetConfigTable(type);
            return configTable.HasDeletedColumn;
        }

        /// <summary>
        /// Lấy đối tượng ConfigTable từ attribute gắn trên class. Nếu không có attribute thì trả ConfigTable mặc định.
        /// </summary>
        private static ConfigTable GetConfigTable(this Type type)
        {
            var configTable = type.GetCustomAttributes(typeof(ConfigTable), true).FirstOrDefault() as ConfigTable;

            if (configTable == null)
            {
                // Trả về object mặc định để tránh null reference trong các helper khác
                configTable = new ConfigTable();
            }

            return configTable;
        }

        /// <summary>
        /// Lấy tên thuộc tính được đánh dấu [Key] - được xem là primary key của entity.
        /// Nếu không có KeyAttribute sẽ ném ArgumentException.
        /// </summary>
        public static string GetKeyName(this Type type)
        {
            var propeties = type.GetProperties();
            var key = propeties.FirstOrDefault(f => f.IsDefined(typeof(KeyAttribute), true));

            if (key == null)
            {
                throw new ArgumentException($"{type.GetTableName()} Không có primarykey");
            }
            ;

            return key.Name;
        }
    }
}
