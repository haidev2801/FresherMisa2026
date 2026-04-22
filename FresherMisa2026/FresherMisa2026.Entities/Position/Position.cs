using FresherMisa2026.Entities.Extensions;
using System;
using System.ComponentModel.DataAnnotations;

namespace FresherMisa2026.Entities.Position
{
    [ConfigTable("Position", false, "PositionCode")]
    /// <summary>
    /// Thông tin vị trí công việc
    /// </summary>
    public class Position : BaseModel
    {
        /// <summary>
        /// Id vị trí
        /// </summary>
        [Key]
        public Guid PositionID { get; set; }

        /// <summary>
        /// Mã vị trí
        /// </summary>
        [IRequired]
        public string? PositionCode { get; set; }

        /// <summary>
        /// Tên vị trí
        /// </summary>
        [IRequired]
        public string? PositionName { get; set; }
    }
}