using Dapper;
using FresherMisa2026.Application.Extensions;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Employee;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;

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

        public async Task<FilterResponse<Employee>> GetEmployeesByFilter(FilterEmployeesRequest request)
        {
            var filterResponse = new FilterResponse<Employee>();
            await OpenConnectionAsync();

            const string store = "Proc_Employee_Filter";

            var parameters = new DynamicParameters();
            parameters.Add("@v_departmentId", request.DepartmentId);
            parameters.Add("@v_positionId", request.PositionId);
            parameters.Add("@v_salaryFrom", request.SalaryFrom);
            parameters.Add("@v_salaryTo", request.SalaryTo);
            parameters.Add("@v_gender", request.Gender);
            parameters.Add("@v_hireDateFrom", request.HireDateFrom?.Date);
            parameters.Add("@v_hireDateTo", request.HireDateTo?.Date);

            using var reader = await _dbConnection.QueryMultipleAsync(
                new CommandDefinition(
                    store,
                    parameters,
                    commandType: CommandType.StoredProcedure));

            var data = (await reader.ReadAsync<Employee>()).ToList();
            var total = await reader.ReadFirstAsync<long>();

            filterResponse.Data = data;
            filterResponse.Total = total;

            return filterResponse;
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