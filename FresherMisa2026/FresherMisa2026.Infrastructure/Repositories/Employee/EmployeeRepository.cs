using Dapper;
using FresherMisa2026.Application.Extensions;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Entities.Employee;
using FresherMisa2026.Entities.Employee.DTO;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Text;

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

        public async Task<IEnumerable<Employee>> FilterEmployeesAsync(EmployeeFilterRequest filterRequest)
        {
            var query = new StringBuilder("SELECT * FROM Employee WHERE 1 = 1");
            var param = new DynamicParameters();

            if (filterRequest.DepartmentId.HasValue)
            {
                query.Append(" AND DepartmentID = @DepartmentID");
                param.Add("@DepartmentID", filterRequest.DepartmentId.Value);
            }

            if (filterRequest.PositionId.HasValue)
            {
                query.Append(" AND PositionID = @PositionID");
                param.Add("@PositionID", filterRequest.PositionId.Value);
            }

            if (filterRequest.SalaryFrom.HasValue)
            {
                query.Append(" AND Salary >= @SalaryFrom");
                param.Add("@SalaryFrom", filterRequest.SalaryFrom.Value);
            }

            if (filterRequest.SalaryTo.HasValue)
            {
                query.Append(" AND Salary <= @SalaryTo");
                param.Add("@SalaryTo", filterRequest.SalaryTo.Value);
            }

            if (filterRequest.Gender.HasValue)
            {
                query.Append(" AND Gender = @Gender");
                param.Add("@Gender", filterRequest.Gender.Value);
            }

            if (filterRequest.HireDateFrom.HasValue)
            {
                query.Append(" AND HireDate >= @HireDateFrom");
                param.Add("@HireDateFrom", filterRequest.HireDateFrom.Value.Date);
            }

            if (filterRequest.HireDateTo.HasValue)
            {
                query.Append(" AND HireDate <= @HireDateTo");
                param.Add("@HireDateTo", filterRequest.HireDateTo.Value.Date);
            }

            return await _dbConnection.QueryAsync<Employee>(query.ToString(), param, commandType: System.Data.CommandType.Text);
        }
    }
}