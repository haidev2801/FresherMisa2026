using Dapper;
using FresherMisa2026.Application.Extensions;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Entities.Employee;
using FresherMisa2026.Entities.Exceptions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;

namespace FresherMisa2026.Infrastructure.Repositories
{
    public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(IConfiguration configuration, IMemoryCache memoryCache) : base(configuration, memoryCache)
        {
        }

        public async Task<Employee> GetEmployeeByEmployeeCodeAsync(string code)
        {
            string query = SQLExtension.GetQuery("Employee.GetByCode");
            var param = new Dictionary<string, object>
            {
                {"@EmployeeCode", code }
            };
            return await _dbConnection.QueryFirstOrDefaultAsync<Employee>(query, param, commandType: System.Data.CommandType.Text);
        }

        public async Task<(IEnumerable<Employee> employees, int TotalRecords)> GetFilter(string? departmentId, string? positionId, decimal? salaryFrom, decimal? salaryTo, int? gender, DateTime? hireDateFrom, DateTime? hireDateTo, int PageSize = 20, int PageIndex = 1)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@v_DepartmentID", string.IsNullOrEmpty(departmentId) ? null : departmentId);
                parameters.Add("@v_PositionID", string.IsNullOrEmpty(positionId) ? null : positionId);
                parameters.Add("@v_SalaryFrom", salaryFrom == 0 ? null : salaryFrom);
                parameters.Add("@v_SalaryTo", salaryTo == 0 ? null : salaryTo);
                parameters.Add("@v_Gender", gender < 0 ? null : gender);
                parameters.Add("@v_HireDateFrom", hireDateFrom == null ? null : hireDateFrom.Value.Date);
                parameters.Add("@v_HireDateTo", hireDateTo == null ? null : hireDateTo.Value.Date);
                parameters.Add("@v_PageSize", PageSize);
                parameters.Add("@v_PageIndex", PageIndex);

                
                var multi = await _dbConnection.QueryMultipleAsync(
                    "Proc_FilterEmployee",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                var employees = await multi.ReadAsync<Employee>();
                var totalRecords = await multi.ReadFirstOrDefaultAsync<int>();

                return (employees, totalRecords);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Lỗi khi lọc nhân viên từ database.", innerException: ex);
            }
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentCodeAsync(string departmentCode)
        {
            string query = SQLExtension.GetQuery("Employee.GetEmployeesByDepartmentCode");
            var parameters = new { DepartmentCode = departmentCode };
            return await _dbConnection.QueryAsync<Employee>(query, parameters);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentId(Guid departmentId)
        {
            string query = SQLExtension.GetQuery("Employee.GetByDepartmentId");
            var param = new Dictionary<string, object>
            {
                {"@DepartmentID", departmentId }
            };
            return await _dbConnection.QueryAsync<Employee>(query, param, commandType: System.Data.CommandType.Text);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByPositionId(Guid positionId)
        {
            string query = SQLExtension.GetQuery("Employee.GetByPositionId");
            var param = new Dictionary<string, object>
            {
                {"@PositionID", positionId }
            };
            return await _dbConnection.QueryAsync<Employee>(query, param, commandType: System.Data.CommandType.Text);
        }
    }
}