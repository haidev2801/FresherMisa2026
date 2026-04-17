using Dapper;
using FresherMisa2026.Application.Extensions;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Employee;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using static Dapper.SqlMapper;

namespace FresherMisa2026.Infrastructure.Repositories
{
    public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<Employee> GetEmployeeByCode(string code)
        {
            string query = SQLExtension.GetQuery("Employee.GetByCode");
            var param = new Dictionary<string, object>
            {
                {"@EmployeeCode", code }
            };
            return await _dbConnection.QueryFirstOrDefaultAsync<Employee>(query, param, commandType: System.Data.CommandType.Text);
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
        public async Task<PagingResponse<Employee>> GetFilterAsync(FilterRequest filterRequest)
        {
            var filterResponse = new PagingResponse<Employee>();
            await OpenConnectionAsync();

            string store = string.Format("Proc_Employee_Filter", _tableName);
            var parameters = new DynamicParameters();
            parameters.Add("@v_departmentId", filterRequest.DepartmentId);
            parameters.Add("@v_positionId", filterRequest.PositionId);
            parameters.Add("@v_salaryFrom", filterRequest.SalaryFrom);
            parameters.Add("@v_salaryTo", filterRequest.SalaryTo);
            parameters.Add("@v_gender", filterRequest.Gender);
            parameters.Add("@v_hireDateFrom", filterRequest.HireDateFrom);
            parameters.Add("@v_hireDateTo", filterRequest.HireDateTo);
            parameters.Add("@v_pageIndex", filterRequest.PageIndex);
            parameters.Add("@v_pageSize", filterRequest.PageSize);

            using var reader = await _dbConnection.QueryMultipleAsync(
               new CommandDefinition(store, parameters, commandType: CommandType.StoredProcedure));

            var data = (await reader.ReadAsync<Employee>()).ToList();
            var total = await reader.ReadFirstAsync<long>();
            filterResponse.Total = total;
            filterResponse.Data = data;

            return filterResponse;
        }
    }
}