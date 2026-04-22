using Dapper;
using FresherMisa2026.Application.Extensions;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Entities.Position;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace FresherMisa2026.Infrastructure.Repositories
{
    /// <summary>
    /// Repository xử lý dữ liệu vị trí
    /// </summary>
    public class PositionRepository : BaseRepository<Position>, IPositionRepository
    {
        /// <summary>
        /// Khởi tạo PositionRepository
        /// </summary>
        /// <param name="configuration">Cấu hình ứng dụng</param>
        /// <param name="memoryCache">Bộ nhớ đệm</param>
        public PositionRepository(IConfiguration configuration,  IMemoryCache memoryCache) : base(configuration, memoryCache)
        {
        }

        /// <summary>
        /// Lấy vị trí theo mã
        /// </summary>
        /// <param name="code">Mã vị trí</param>
        /// <returns>Vị trí tìm thấy</returns>
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