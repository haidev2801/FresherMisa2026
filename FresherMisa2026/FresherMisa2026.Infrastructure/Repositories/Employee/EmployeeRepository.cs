using Dapper;
using FresherMisa2026.Application.Extensions;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Entities.Employee;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Text;

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

        public async Task<(long Total, IEnumerable<Employee> Data)> FilterEmployees(EmployeeFilterRequest request)
        {
            var sql = new StringBuilder("FROM employee WHERE 1=1");
            var parameters = new DynamicParameters();

            if (request.DepartmentId.HasValue)
            {
                sql.Append(" AND DepartmentID = @DepartmentID");
                parameters.Add("@DepartmentID", request.DepartmentId);
            }

            if (request.PositionId.HasValue)
            {
                sql.Append(" AND PositionID = @PositionID");
                parameters.Add("@PositionID", request.PositionId);
            }

            if (request.SalaryFrom.HasValue)
            {
                sql.Append(" AND Salary >= @SalaryFrom");
                parameters.Add("@SalaryFrom", request.SalaryFrom);
            }

            if (request.SalaryTo.HasValue)
            {
                sql.Append(" AND Salary <= @SalaryTo");
                parameters.Add("@SalaryTo", request.SalaryTo);
            }

            if (request.Gender.HasValue)
            {
                sql.Append(" AND Gender = @Gender");
                parameters.Add("@Gender", request.Gender);
            }

            if (request.HireDateFrom.HasValue)
            {
                sql.Append(" AND CreatedDate >= @HireDateFrom");
                parameters.Add("@HireDateFrom", request.HireDateFrom);
            }

            if (request.HireDateTo.HasValue)
            {
                sql.Append(" AND CreatedDate <= @HireDateTo");
                parameters.Add("@HireDateTo", request.HireDateTo);
            }

            // query total
            var totalSql = $"SELECT COUNT(*) {sql}";
            var total = await _dbConnection.ExecuteScalarAsync<long>(totalSql, parameters);

            // paging
            var offset = (request.PageIndex - 1) * request.PageSize;
            parameters.Add("@Offset", offset);
            parameters.Add("@PageSize", request.PageSize);

            var dataSql = $"SELECT * {sql} LIMIT @PageSize OFFSET @Offset";

            var data = await _dbConnection.QueryAsync<Employee>(dataSql, parameters);

            return (total, data);
        }
    }
}