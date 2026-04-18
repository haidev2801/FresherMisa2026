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

        public async Task<int> GetCountEmployeesByDepartmentId(string departmentCode)
        {
            await OpenConnectionAsync();

            string store = string.Format("Proc_Department_GetEmployeeCount", _tableName);
            var parameters = new DynamicParameters();
            parameters.Add("@v_departmentCode", departmentCode);

            using var reader = await _dbConnection.QueryMultipleAsync(
               new CommandDefinition(store, parameters, commandType: CommandType.StoredProcedure));

            var total = await reader.ReadFirstAsync<long>();
            return (int)total;
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

        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentId(string departmentCode)
        {
            await OpenConnectionAsync();

            string store = string.Format("Proc_Department_GetEmployees", _tableName);
            var parameters = new DynamicParameters();
            parameters.Add("@v_departmentCode", departmentCode);

            using var reader = await _dbConnection.QueryMultipleAsync(
               new CommandDefinition(store, parameters, commandType: CommandType.StoredProcedure));

            var data = (await reader.ReadAsync<Employee>()).ToList();
            return data;
        }
    }
}
