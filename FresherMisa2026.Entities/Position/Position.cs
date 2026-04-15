using FresherMisa2026.Entities.Extensions;
using System;
using System.ComponentModel.DataAnnotations;

namespace FresherMisa2026.Entities.Position
{
    [ConfigTable("Position", false, "PositionCode")]
    public class Position : BaseModel
    {
        /// <summary>
        /// ID vị trí
        /// </summary>
        [Key]
        public Guid PositionID { get; set; }

        /// <summary>
        /// Mã vị trí
        /// </summary>
        public string PositionCode { get; set; }

        /// <summary>
        /// Tên vị trí
        /// </summary>
        public string PositionName { get; set; }
    }
}