using Dapper;
using FresherMisa2026.Application.Extensions;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Entities.Employee;
using FresherMisa2026.Entities.Employee.DTO;
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

        public async Task<IEnumerable<Employee>> FilterEmployeesAsync(EmployeeFilterRequest request)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@v_DepartmentID", request.DepartmentId);
            parameters.Add("@v_PositionID", request.PositionId);
            parameters.Add("@v_SalaryFrom", request.SalaryFrom);
            parameters.Add("@v_SalaryTo", request.SalaryTo);
            parameters.Add("@v_Gender", request.Gender);
            parameters.Add("@v_HireDateFrom", request.HireDateFrom);
            parameters.Add("@v_HireDateTo", request.HireDateTo);

            return await _dbConnection.QueryAsync<Employee>(
                "Proc_Employee_Filter",
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<int> CountEmployeesByDepartmentIdAsync(Guid departmentId)
        {
            const string sql = "SELECT COUNT(*) FROM Employee WHERE DepartmentID = @DepartmentID";
            return await _dbConnection.ExecuteScalarAsync<int>(sql, new { DepartmentID = departmentId });
        }
    }
}