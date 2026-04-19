using FresherMisa2026.Application.Interfaces;
using FresherMisa2026.Application.Interfaces.Services;
using FresherMisa2026.Entities.Position;
using Microsoft.Extensions.Caching.Memory;


namespace FresherMisa2026.Application.Services
{
    public class PositionService : BaseService<Position>, IPositionService
    {
    
        public PositionService(
            IBaseRepository<Position> baseRepository,
            IMemoryCache cache
            ) : base(baseRepository, cache)
        {
        }
    }
}