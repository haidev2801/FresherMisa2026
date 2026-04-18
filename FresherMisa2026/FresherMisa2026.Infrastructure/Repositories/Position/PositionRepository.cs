using Dapper;
using FresherMisa2026.Application.Extensions;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Entities.Position;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;

namespace FresherMisa2026.Infrastructure.Repositories
{
    /// <summary>
    /// Repository cho Position
    /// </summary>
    /// Created by: Phuong (17/04/2026)
    public class PositionRepository : BaseRepository<Position>, IPositionRepository
    {
        public PositionRepository(IConfiguration configuration, IMemoryCache memoryCache) : base(configuration, memoryCache)
        {
        }

        /// <summary>
        /// Lấy position theo code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        /// Created by: Phuong (17/04/2026)
        public async Task<Position> GetPositionByCode(string code)
        {
            string query = SQLExtension.GetQuery("Position.GetByCode");
            var param = new Dictionary<string, object>
            {
                {"@PositionCode", code }
            };
            return await _dbConnection.QueryFirstOrDefaultAsync<Position>(query, param, commandType: System.Data.CommandType.Text);
        }
    }
}