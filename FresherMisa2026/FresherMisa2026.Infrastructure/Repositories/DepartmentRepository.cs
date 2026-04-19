using Dapper;
using FresherMisa2026.Application.Extensions;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Entities.Department;
using FresherMisa2026.Entities.Employee;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;

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
        public async Task<Department> GetDepartmentByCode(string code)
        {
            string query = SQLExtension.GetQuery("Department.GetByCode");
            var @param = new Dictionary<string, object>
            {
                {"@DepartmentCode", code }
            };
            using (var dbConnection = await OpenConnectionAsync())
            {
                return await dbConnection.QueryFirstOrDefaultAsync<Department>(query, @param, commandType: CommandType.Text);
            }
        }

        /// <summary>
        /// Lấy danh sách nhân viên theo department code
        /// </summary>
        /// <param name="departmentCode"></param>
        /// <returns>Danh sách nhân viên tìm thấy hoặc rỗng</returns>
        /// CREATED BY: tannn (18/04/2026)
        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentCode(string departmentCode)
        {
            string query = SQLExtension.GetQuery("Department.GetEmployeesByCode");
            var @param = new Dictionary<string, object>
            {
                {"@DepartmentCode", departmentCode }
            };
            using (var dbConnection = await OpenConnectionAsync())
            {
                return await dbConnection.QueryAsync<Employee>(query, @param, commandType: CommandType.Text);
            }
        }

        /// <summary>
        /// Lấy số lượng nhân viên theo department code
        /// </summary>
        /// <param name="departmentCode"></param>
        /// <returns>Số lượng nhân viên tìm thấy</returns>
        /// CREATED BY: tannn (18/04/2026)
        public async Task<int> CountEmployeesByDepartmentCode(string departmentCode)
        {
            string query = SQLExtension.GetQuery("Department.CountEmployeesByCode");
            var @param = new Dictionary<string, object>
            {
                {"@DepartmentCode", departmentCode }
            };
            using (var dbConnection = await OpenConnectionAsync())
            {
                return await dbConnection.QueryFirstOrDefaultAsync<int>(query, @param, commandType: CommandType.Text);
            }
        }
    }
}
