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

        public async Task<IEnumerable<Employee>> FilterEmployees(
            Guid? departmentId,
            Guid? positionId,
            decimal? salaryFrom,
            decimal? salaryTo,
            int? gender,
            DateTime? hireDateFrom,
            DateTime? hireDateTo)
        {
            var sql = "SELECT * FROM Employee WHERE 1=1";
            var param = new DynamicParameters();

            if (departmentId.HasValue)
            {
                sql += " AND DepartmentID = @DepartmentID";
                param.Add("@DepartmentID", departmentId.Value.ToString());
            }

            if (positionId.HasValue)
            {
                sql += " AND PositionID = @PositionID";
                param.Add("@PositionID", positionId.Value.ToString());
            }

            if (salaryFrom.HasValue)
            {
                sql += " AND Salary >= @SalaryFrom";
                param.Add("@SalaryFrom", salaryFrom.Value);
            }

            if (salaryTo.HasValue)
            {
                sql += " AND Salary <= @SalaryTo";
                param.Add("@SalaryTo", salaryTo.Value);
            }

            if (gender.HasValue)
            {
                sql += " AND Gender = @Gender";
                param.Add("@Gender", gender.Value);
            }

            if (hireDateFrom.HasValue)
            {
                sql += " AND HireDateFrom >= @HireDateFrom";
                param.Add("@HireDateFrom", hireDateFrom.Value);
            }

            if (hireDateTo.HasValue)
            {
                sql += " AND HireDateTo <= @HireDateTo";
                param.Add("@HireDateTo", hireDateTo.Value);
            }

            sql += " ORDER BY EmployeeCode ASC";

            return await _dbConnection.QueryAsync<Employee>(sql, param, commandType: System.Data.CommandType.Text);
        }
    }
}