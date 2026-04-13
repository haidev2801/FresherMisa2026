using FresherMisa2026.Entities.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FresherMisa2026.Entities.Department
{
    /// <summary>
    /// Entity đại diện cho bảng Department trong database.
    /// Gắn attribute [ConfigTable] để chỉ định tên bảng và các thuộc tính mapping khác.
    /// </summary>
    [ConfigTable("Department", false, "DepartmentCode")]
    public class Department : BaseModel
    {
        /// <summary>
        /// ID phòng ban (primary key). Được đánh dấu [Key] để helper reflection nhận diện.
        /// </summary>
        [Key]
        public Guid DepartmentID { get; set; }

        /// <summary>
        /// Mã phòng ban (unique theo ConfigTable.UniqueColumns)
        /// </summary>
        public string DepartmentCode { get; set; }

        /// <summary>
        /// Tên phòng ban
        /// </summary>
        public string DepartmentName { get; set; }

        /// <summary>
        /// Mô tả phòng ban
        /// </summary>
        public string Description { get; set; }
    }
}
