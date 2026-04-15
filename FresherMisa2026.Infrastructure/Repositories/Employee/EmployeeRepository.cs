using Dapper;
using FresherMisa2026.Application.Extensions;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Entities.Employee;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace FresherMisa2026.Infrastructure.Repositories
{
    /// <summary>
    /// Repository for Employee entity
    /// </summary>
    /// Created By: dvhai (15/04/2026)
    public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(IConfiguration configuration) : base(configuration)
        {

        }

        /// <summary>
        /// Lấy employee theo code
        /// </summary>
        /// <param name="code">Mã employee</param>
        /// <returns>Employee tìm thấy hoặc null</returns>
        /// CREATED BY: dvhai (15/04/2026)
        public async Task<Employee> GetEmployeeByCode(string code)
        {
            string query = SQLExtension.GetQuery("Employee.GetByCode");
            var @param = new Dictionary<string, object>
            {
                {"@EmployeeCode", code }
            };
            return await _dbConnection.QueryFirstOrDefaultAsync<Employee>(query, @param, commandType: System.Data.CommandType.Text);
        }

        /// <summary>
        /// Lấy danh sách employee theo department id
        /// </summary>
        /// <param name="departmentId">ID phòng ban</param>
        /// <returns>Danh sách employee</returns>
        /// CREATED BY: dvhai (15/04/2026)
        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentId(Guid departmentId)
        {
            string query = SQLExtension.GetQuery("Employee.GetByDepartmentId");
            var @param = new Dictionary<string, object>
            {
                {"@DepartmentID", departmentId }
            };
            return await _dbConnection.QueryAsync<Employee>(query, @param, commandType: System.Data.CommandType.Text);
        }

        /// <summary>
        /// Lấy danh sách employee theo position id
        /// </summary>
        /// <param name="positionId">ID vị trí</param>
        /// <returns>Danh sách employee</returns>
        /// CREATED BY: dvhai (15/04/2026)
        public async Task<IEnumerable<Employee>> GetEmployeesByPositionId(Guid positionId)
        {
            string query = SQLExtension.GetQuery("Employee.GetByPositionId");
            var @param = new Dictionary<string, object>
            {
                {"@PositionID", positionId }
            };
            return await _dbConnection.QueryAsync<Employee>(query, @param, commandType: System.Data.CommandType.Text);
        }
    }
}