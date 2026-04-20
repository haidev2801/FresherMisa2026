using FresherMisa2026.Application.Dtos.Position;
using FresherMisa2026.Entities.Position;

namespace FresherMisa2026.Application.Mappers.Positions
{
    public static class ToPositionMapper
    {
        public static Position ToPositionFromCreateDto(this CreatePositionDto dto)
        {
            return new Position
            {
                PositionID = dto.PositionID,
                PositionName = dto.PositionName,
                PositionCode = dto.PositionCode,
            };
        }
    }
}
