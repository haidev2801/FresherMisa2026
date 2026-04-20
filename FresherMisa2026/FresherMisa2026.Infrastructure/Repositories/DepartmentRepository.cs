using Dapper;
using FresherMisa2026.Application.Extensions;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Entities.Department;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
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
        public DepartmentRepository(IConfiguration configuration, IMemoryCache cache)
            : base(configuration, cache)
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
            await using var connection = await CreateAndOpenConnectionAsync();
            return await connection.QueryFirstOrDefaultAsync<Department>(query, @param, commandType: System.Data.CommandType.Text);
        }

        public async Task<int> GetEmployeeCountByDepartmentId(Guid departmentId)
        {
            string query = SQLExtension.GetQuery("Employee.CountByDepartmentId");
            var @param = new Dictionary<string, object>
            {
                {"@DepartmentID", departmentId }
            };

            await using var connection = await CreateAndOpenConnectionAsync();
            return await connection.QueryFirstOrDefaultAsync<int>(query, @param, commandType: System.Data.CommandType.Text);
        }
    }
}
