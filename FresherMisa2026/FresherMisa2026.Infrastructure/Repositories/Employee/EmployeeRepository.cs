using Dapper;
using FresherMisa2026.Application.Extensions;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Entities.Employee;
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

        public async Task<IEnumerable<Employee>> FilterEmployees(
            Guid? departmentId,
            Guid? positionId,
            decimal? salaryFrom,
            decimal? salaryTo,
            int? gender,
            DateTime? hireDateFrom,
            DateTime? hireDateTo)
        {
            var query = new StringBuilder("SELECT * FROM Employee WHERE 1 = 1");
            var parameters = new DynamicParameters();

            if (departmentId.HasValue && departmentId.Value != Guid.Empty)
            {
                query.Append(" AND DepartmentID = @DepartmentID");
                parameters.Add("@DepartmentID", departmentId.Value);
            }

            if (positionId.HasValue && positionId.Value != Guid.Empty)
            {
                query.Append(" AND PositionID = @PositionID");
                parameters.Add("@PositionID", positionId.Value);
            }

            if (salaryFrom.HasValue)
            {
                query.Append(" AND Salary >= @SalaryFrom");
                parameters.Add("@SalaryFrom", salaryFrom.Value);
            }

            if (salaryTo.HasValue)
            {
                query.Append(" AND Salary <= @SalaryTo");
                parameters.Add("@SalaryTo", salaryTo.Value);
            }

            if (gender.HasValue)
            {
                query.Append(" AND Gender = @Gender");
                parameters.Add("@Gender", gender.Value);
            }

            if (hireDateFrom.HasValue)
            {
                query.Append(" AND HireDate >= @HireDateFrom");
                parameters.Add("@HireDateFrom", hireDateFrom.Value.Date);
            }

            if (hireDateTo.HasValue)
            {
                query.Append(" AND HireDate <= @HireDateTo");
                parameters.Add("@HireDateTo", hireDateTo.Value.Date);
            }

            return await _dbConnection.QueryAsync<Employee>(query.ToString(), parameters, commandType: System.Data.CommandType.Text);
        }
    }
}