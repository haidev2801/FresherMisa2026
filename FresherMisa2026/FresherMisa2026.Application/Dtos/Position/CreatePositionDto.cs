namespace FresherMisa2026.Application.Dtos.Position
{
    public class CreatePositionDto
    {
        public Guid PositionID { get; set; }
        public string PositionCode { get; set; } = string.Empty;
        public string PositionName { get; set; } = string.Empty;
    }
}
