using FresherMisa2026.Application.Dtos.Position;
using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Position;

namespace FresherMisa2026.Application.Interfaces.Services
{
    public interface IPositionService : IBaseService<Position>
    {
        /// <summary>
        /// Lấy position theo code
        /// </summary>
        /// <param name="code">Mã position</param>
        /// <returns>Position tìm thấy</returns>
        Task<Position> GetPositionByCodeAsync(string code);
        Task<ServiceResponse> CreatePositionDtoAsync(CreatePositionDto dto);
        Task<ServiceResponse> UpdatePositionDtoAsync(Guid id, UpdatePositionDto dto);
    }
}