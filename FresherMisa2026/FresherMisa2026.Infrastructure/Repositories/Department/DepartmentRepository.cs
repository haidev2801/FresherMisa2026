using Dapper;
using FresherMisa2026.Application.Extensions;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Entities.Department;
using FresherMisa2026.Entities.Employee;
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
        public DepartmentRepository(IConfiguration configuration, IMemoryCache memoryCache) : base(configuration, memoryCache)
        {

        }

        /// <summary>
        /// Lấy department theo code
        /// </summary>
        /// <param name="code">Mã department</param>
        /// <returns>Department tìm thấy hoặc null</returns>
        /// CREATED BY: dvhai (09/04/2026)
        public async Task<Department?> GetDepartmentByCode(string code)
        {
            string query = SQLExtension.GetQuery("Department.GetByCode");
            var @param = new Dictionary<string, object>
            {
                {"@DepartmentCode", code }
            };
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            return await connection.QueryFirstOrDefaultAsync<Department>(query, @param, commandType: System.Data.CommandType.Text);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentCode(string code)
        {
            var param = new DynamicParameters();
            param.Add("v_DepartmentCode", code);

            await using var connection = CreateConnection();
            await connection.OpenAsync();

            return await connection.QueryAsync<Employee>("Proc_GetEmployeesByDepartmentCode", param, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<long> CountEmployeesByDepartmentIdAsync(Guid departmentId)
        {
            const string query = "SELECT COUNT(*) FROM employee WHERE DepartmentID = @DepartmentID";

            await using var connection = CreateConnection();
            await connection.OpenAsync();

            return await connection.ExecuteScalarAsync<long>(
                query,
                new { DepartmentID = departmentId.ToString() },
                commandType: System.Data.CommandType.Text);
        }
    }
}
