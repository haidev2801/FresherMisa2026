using Dapper;
using FresherMisa2026.Application.Extensions;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Entities.Employee;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;

namespace FresherMisa2026.Infrastructure.Repositories
{
    public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(IConfiguration configuration, IMemoryCache cache) : base(configuration, cache)
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
            var sql = "SELECT * FROM employee WHERE 1=1";
            var parameters = new DynamicParameters();

            if (request.DepartmentId.HasValue)
            {
                sql += " AND DepartmentID = @DepartmentID";
                parameters.Add("@DepartmentID", request.DepartmentId);
            }

            if (request.PositionId.HasValue)
            {
                sql += " AND PositionID = @PositionID";
                parameters.Add("@PositionID", request.PositionId);
            }

            if (request.SalaryFrom.HasValue)
            {
                sql += " AND Salary >= @SalaryFrom";
                parameters.Add("@SalaryFrom", request.SalaryFrom);
            }

            if (request.SalaryTo.HasValue)
            {
                sql += " AND Salary <= @SalaryTo";
                parameters.Add("@SalaryTo", request.SalaryTo);
            }

            if (request.Gender.HasValue)
            {
                sql += " AND Gender = @Gender";
                parameters.Add("@Gender", request.Gender);
            }

            if (request.HireDateFrom.HasValue)
            {
                sql += " AND CreatedDate >= @HireDateFrom";
                parameters.Add("@HireDateFrom", request.HireDateFrom);
            }

            if (request.HireDateTo.HasValue)
            {
                sql += " AND CreatedDate <= @HireDateTo";
                parameters.Add("@HireDateTo", request.HireDateTo);
            }

            return await _dbConnection.QueryAsync<Employee>(sql, parameters);
        }
    }
}