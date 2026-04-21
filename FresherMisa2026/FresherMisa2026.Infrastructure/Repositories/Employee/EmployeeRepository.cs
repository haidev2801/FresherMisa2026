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

        public async Task<IEnumerable<Employee>> GetEmployeesByPositionId(Guid positionId)
        {
            string query = SQLExtension.GetQuery("Employee.GetByPositionId");
            var param = new Dictionary<string, object>
            {
                {"@PositionID", positionId }
            };
            return await _dbConnection.QueryAsync<Employee>(query, param, commandType: System.Data.CommandType.Text);
        }


        // Thực hiện lọc nhân viên theo các tiêu chí được cung cấp trong request
        public async Task<FilterResponse<Employee>> GetEmployeesByFilter(FilterEmployeeRq request)
        {
            var filterResponse = new FilterResponse<Employee>();
            await OpenConnectionAsync();

            const string store = "Proc_Employee_Filter";

            var parameters = new DynamicParameters();
            parameters.Add("p_DepartmentID", request.DepartmentID);
            parameters.Add("p_PositionID", request.PositionID);
            parameters.Add("p_SalaryFrom", request.SalaryFrom);
            parameters.Add("p_SalaryTo", request.SalaryTo);
            parameters.Add("p_Gender", request.Gender);
            parameters.Add("p_HireDateFrom", request.HireDateFrom?.Date);
            parameters.Add("p_HireDateTo", request.HireDateTo?.Date);
            parameters.Add("p_pageSize", request.PageSize);
            parameters.Add("p_pageIndex", request.PageIndex);

            using var reader = await _dbConnection.QueryMultipleAsync(
                new CommandDefinition(
                    store,
                    parameters,
                    commandType: CommandType.StoredProcedure));

            var data = (await reader.ReadAsync<Employee>()).ToList();
            var total = await reader.ReadFirstAsync<long>();

            filterResponse.Data = data;
            filterResponse.Total = total;
            filterResponse.PageSize = request.PageSize ?? 0;
            filterResponse.PageIndex = request.PageIndex ?? 0;

            return filterResponse;
        }
    }
}