using Dapper;
using FresherMisa2026.Application.Extensions;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Entities.Position;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace FresherMisa2026.Infrastructure.Repositories
{
    public class PositionRepository : BaseRepository<Position>, IPositionRepository
    {
        public PositionRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<Position> GetPositionByCode(string code)
        {
            string query = SQLExtension.GetQuery("Position.GetByCode");
            using var conn = await CreateConnectionAsync();
            var param = new Dictionary<string, object>
            {
                {"@PositionCode", code }
            };
            return await conn.QueryFirstOrDefaultAsync<Position>(query, param,
                commandType: CommandType.Text);
        }
    }
}