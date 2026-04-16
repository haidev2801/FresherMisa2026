using FresherMisa2026.Entities.Position;

namespace FresherMisa2026.Application.Interfaces.Repositories
{
    public interface IPositionRepository : IBaseRepository<Position>
    {
        Task<Position> GetPositionByCode(string code);
    }
}