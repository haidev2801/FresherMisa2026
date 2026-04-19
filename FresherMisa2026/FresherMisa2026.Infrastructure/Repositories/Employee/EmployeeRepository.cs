using Dapper;
using FresherMisa2026.Application.DTOs.Employee;
using FresherMisa2026.Application.Extensions;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Entities.Employee;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;

namespace FresherMisa2026.Infrastructure.Repositories
{
    public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
    {
        private sealed class TotalCountResult
        {
            public long TotalCount { get; set; }
        }

        public EmployeeRepository(IConfiguration configuration, IMemoryCache memoryCache) : base(configuration, memoryCache)
        {
        }

        public async Task<Employee> GetEmployeeByCode(string code)
        {
            string query = SQLExtension.GetQuery("Employee.GetByCode");
            var param = new Dictionary<string, object>
            {
                {"@EmployeeCode", code }
            };
            await using var connection = await CreateOpenConnectionAsync();
            return await connection.QueryFirstOrDefaultAsync<Employee>(query, param, commandType: System.Data.CommandType.Text);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentId(Guid departmentId)
        {
            string query = SQLExtension.GetQuery("Employee.GetByDepartmentId");
            var param = new Dictionary<string, object>
            {
                {"@DepartmentID", departmentId }
            };
            await using var connection = await CreateOpenConnectionAsync();
            return await connection.QueryAsync<Employee>(query, param, commandType: System.Data.CommandType.Text);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByPositionId(Guid positionId)
        {
            string query = SQLExtension.GetQuery("Employee.GetByPositionId");
            var param = new Dictionary<string, object>
            {
                {"@PositionID", positionId }
            };
            await using var connection = await CreateOpenConnectionAsync();
            return await connection.QueryAsync<Employee>(query, param, commandType: System.Data.CommandType.Text);
        }

        public async Task<(long Total, IEnumerable<Employee> Data)> FilterEmployeesAsync(EmployeeFilterRequest filterRequest)
        {
            var pageSize = filterRequest.PageSize <= 0 ? 20 : filterRequest.PageSize;
            var pageIndex = filterRequest.PageIndex <= 0 ? 1 : filterRequest.PageIndex;
            var offset = (pageIndex - 1) * pageSize;

            var parameters = new DynamicParameters();
            parameters.Add("p_DepartmentID", filterRequest.DepartmentId?.ToString());
            parameters.Add("p_PositionID", filterRequest.PositionId?.ToString());
            parameters.Add("p_SalaryFrom", filterRequest.SalaryFrom);
            parameters.Add("p_SalaryTo", filterRequest.SalaryTo);
            parameters.Add("p_Gender", filterRequest.Gender);
            parameters.Add("p_HireDateFrom", filterRequest.HireDateFrom);
            parameters.Add("p_HireDateTo", filterRequest.HireDateTo);
            parameters.Add("p_Limit", pageSize);
            parameters.Add("p_Offset", offset);
            parameters.Add("p_OrderBy", filterRequest.OrderBy);

            await using var connection = await CreateOpenConnectionAsync();

            using var reader = await connection.QueryMultipleAsync(
                new CommandDefinition("Proc_Employee_FilterPaging_2", parameters, commandType: CommandType.StoredProcedure));

            var data = (await reader.ReadAsync<Employee>()).ToList();
            var totalRow = await reader.ReadFirstOrDefaultAsync<TotalCountResult>();
            var total = totalRow?.TotalCount ?? 0;

            return (total, data);
        }
    }
}