using Dapper;
using FresherMisa2026.Application.Extensions;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Entities.Department;
using FresherMisa2026.Entities.Employee;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace FresherMisa2026.Infrastructure.Repositories
{
    /// <summary>
    /// Repository for Department entity
    /// </summary>
    /// Created By: dvhai (09/04/2026)
    public class DepartmentRepository : BaseRepository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(IConfiguration configuration) : base(configuration)
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
            return await _dbConnection.QueryFirstOrDefaultAsync<Department>(query, @param, commandType: System.Data.CommandType.Text);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentCodeAsync(string code)
        {
            const string query = @"
                SELECT e.*
                FROM Employee e
                INNER JOIN Department d ON e.DepartmentID = d.DepartmentID
                WHERE d.DepartmentCode = @DepartmentCode";

            return await _dbConnection.QueryAsync<Employee>(
                query,
                new { DepartmentCode = code },
                commandType: CommandType.Text);
        }

        public async Task<int> GetEmployeeCountByDepartmentCodeAsync(string code)
        {
            const string query = @"
                SELECT COUNT(*)
                FROM Employee e
                INNER JOIN Department d ON e.DepartmentID = d.DepartmentID
                WHERE d.DepartmentCode = @DepartmentCode";

            return await _dbConnection.ExecuteScalarAsync<int>(
                query,
                new { DepartmentCode = code },
                commandType: CommandType.Text);
        }
    }
}
