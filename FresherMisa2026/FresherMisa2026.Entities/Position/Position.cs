using FresherMisa2026.Entities.Extensions;
using System;
using System.ComponentModel.DataAnnotations;

namespace FresherMisa2026.Entities.Position
{
    [ConfigTable("Position", false, "PositionCode")]
    public class Position : BaseModel
    {
        [Key]
        public Guid PositionID { get; set; }

        [IRequired]
        [Display(Name = "Mã chức vụ")]
        public string PositionCode { get; set; } = string.Empty;

        [IRequired]
        [Display(Name = "Tên chức vụ")]
        public string PositionName { get; set; } = string.Empty;
    }
}