using Dapper;
using FresherMisa2026.Application.Extensions;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Entities.Position;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace FresherMisa2026.Infrastructure.Repositories
{
    /// <summary>
    /// Repository for Position entity
    /// </summary>
    /// Created By: dvhai (15/04/2026)
    public class PositionRepository : BaseRepository<Position>, IPositionRepository
    {
        public PositionRepository(IConfiguration configuration) : base(configuration)
        {

        }

        /// <summary>
        /// Lấy position theo code
        /// </summary>
        /// <param name="code">Mã position</param>
        /// <returns>Position tìm thấy hoặc null</returns>
        /// CREATED BY: dvhai (15/04/2026)
        public async Task<Position> GetPositionByCode(string code)
        {
            string query = SQLExtension.GetQuery("Position.GetByCode");
            var @param = new Dictionary<string, object>
            {
                {"@PositionCode", code }
            };
            return await _dbConnection.QueryFirstOrDefaultAsync<Position>(query, @param, commandType: System.Data.CommandType.Text);
        }
    }
}