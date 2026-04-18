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
        public string? PositionCode { get; set; }
        [IRequired]
        public string? PositionName { get; set; }
    }
}