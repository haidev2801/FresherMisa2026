using Dapper;
using FresherMisa2026.Application.Extensions;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Entities.Position;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using MySqlConnector;
using System.Collections.Generic;

namespace FresherMisa2026.Infrastructure.Repositories
{
    public class PositionRepository : BaseRepository<Position>, IPositionRepository
    {
        public PositionRepository(IConfiguration configuration, IMemoryCache cache) : base(configuration, cache)
        {
        }

        public async Task<Position> GetPositionByCode(string code)
        {
            string query = SQLExtension.GetQuery("Position.GetByCode");
            var param = new Dictionary<string, object>
            {
                {"@PositionCode", code }
            };
            using var conn = await GetOpenConnectionAsync();
            try
            {
                return await conn.QueryFirstOrDefaultAsync<Position>(query, param, commandType: System.Data.CommandType.Text);
            }
            catch (MySqlException mex)
            {
                if (mex.Number == 1062)
                {
                    throw new FresherMisa2026.Infrastructure.Exceptions.DuplicateKeyException(null, mex.Message);
                }

                throw;
            }
        }
    }
}