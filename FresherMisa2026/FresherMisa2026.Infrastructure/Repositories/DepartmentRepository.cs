using Dapper;
using FresherMisa2026.Application.Extensions;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Entities.Department;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Text;

namespace FresherMisa2026.Infrastructure.Repositories
{
    /// <summary>
    /// Repository for Department entity
    /// </summary>
    /// Created By: dvhai (09/04/2026)
    public class DepartmentRepository : BaseRepository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(IConfiguration configuration, IMemoryCache cache) : base(configuration, cache)
        {

        }

        /// <summary>
        /// Lấy department theo code
        /// </summary>
        /// <param name="code">Mã department</param>
        /// <returns>Department tìm thấy hoặc null</returns>
        /// CREATED BY: dvhai (09/04/2026)
        public async Task<Department> GetDepartmentByCode(string code)
        {
            string query = SQLExtension.GetQuery("Department.GetByCode");
            var @param = new Dictionary<string, object>
            {
                {"@DepartmentCode", code }
            };

            using var conn = await GetOpenConnectionAsync();
            try
            {
                return await conn.QueryFirstOrDefaultAsync<Department>(query, @param, commandType: System.Data.CommandType.Text);
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
