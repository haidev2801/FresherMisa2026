using Dapper;
using FresherMisa2026.Application.Extensions;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Entities.Employee;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

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

        public async Task<IEnumerable<Employee>> FilterEmployees(EmployeeFilterRequest request)
        {
            string query = SQLExtension.GetQuery("Employee.Filter");
            var param = new DynamicParameters();

            param.Add("@DepartmentID", request.DepartmentId?.ToString());
            param.Add("@PositionID", request.PositionId?.ToString());
            param.Add("@SalaryFrom", request.SalaryFrom);
            param.Add("@SalaryTo", request.SalaryTo);
            param.Add("@Gender", request.Gender);
            param.Add("@HireDateFrom", request.HireDateFrom?.Date);
            param.Add("@HireDateTo", request.HireDateTo?.Date);

            return await _dbConnection.QueryAsync<Employee>(query, param, commandType: System.Data.CommandType.Text);
        }
    }
}
