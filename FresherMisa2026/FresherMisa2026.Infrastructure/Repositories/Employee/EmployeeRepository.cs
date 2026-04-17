using Dapper;
using FresherMisa2026.Application.Extensions;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Entities.Employee;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;
using System;
using Dapper;
using System.Collections.Generic;
using System.Text;

namespace FresherMisa2026.Infrastructure.Repositories
{
    public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(IConfiguration configuration, IMemoryCache memoryCache = null) : base(configuration, memoryCache)
        {
        }

        public async Task<Employee> GetEmployeeByCode(string code)
        {
            string query = SQLExtension.GetQuery("Employee.GetByCode");
            var param = new Dictionary<string, object>
            {
                {"@EmployeeCode", code }
            };

            using var connection = await OpenConnectionAsync();
            return await connection.QueryFirstOrDefaultAsync<Employee>(query, param, commandType: System.Data.CommandType.Text);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentId(Guid departmentId)
        {
            string query = SQLExtension.GetQuery("Employee.GetByDepartmentId");
            var param = new Dictionary<string, object>
            {
                {"@DepartmentID", departmentId }
            };

            using var connection = await OpenConnectionAsync();
            return await connection.QueryAsync<Employee>(query, param, commandType: System.Data.CommandType.Text);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByPositionId(Guid positionId)
        {
            string query = SQLExtension.GetQuery("Employee.GetByPositionId");
            var param = new Dictionary<string, object>
            {
                {"@PositionID", positionId }
            };

            using var connection = await OpenConnectionAsync();
            return await connection.QueryAsync<Employee>(query, param, commandType: System.Data.CommandType.Text);
        }

        public async Task<IEnumerable<Employee>> FilterEmployees(Guid? departmentId, Guid? positionId, decimal? salaryFrom, decimal? salaryTo, int? gender, DateTime? hireDateFrom, DateTime? hireDateTo)
        {
            // Build dynamic query
            var sql = new StringBuilder(SQLExtension.GetQuery("Employee.FilterBase"));

            var parameters = new DynamicParameters();

            if (departmentId.HasValue && departmentId != Guid.Empty)
            {
                sql.Append(" AND DepartmentID = @DepartmentID");
                parameters.Add("@DepartmentID", departmentId.Value);
            }

            if (positionId.HasValue && positionId != Guid.Empty)
            {
                sql.Append(" AND PositionID = @PositionID");
                parameters.Add("@PositionID", positionId.Value);
            }

            if (salaryFrom.HasValue)
            {
                sql.Append(" AND Salary >= @SalaryFrom");
                parameters.Add("@SalaryFrom", salaryFrom.Value);
            }

            if (salaryTo.HasValue)
            {
                sql.Append(" AND Salary <= @SalaryTo");
                parameters.Add("@SalaryTo", salaryTo.Value);
            }

            if (gender.HasValue)
            {
                sql.Append(" AND Gender = @Gender");
                parameters.Add("@Gender", gender.Value);
            }

            // Use CreatedDate as hire date if provided
            if (hireDateFrom.HasValue)
            {
                sql.Append(" AND CreatedDate >= @HireDateFrom");
                parameters.Add("@HireDateFrom", hireDateFrom.Value.Date);
            }

            if (hireDateTo.HasValue)
            {
                sql.Append(" AND CreatedDate <= @HireDateTo");
                parameters.Add("@HireDateTo", hireDateTo.Value.Date);
            }

            var finalSql = sql.ToString();

            using var connection = await OpenConnectionAsync();
            return await connection.QueryAsync<Employee>(finalSql, parameters, commandType: System.Data.CommandType.Text);
        }
    }
}