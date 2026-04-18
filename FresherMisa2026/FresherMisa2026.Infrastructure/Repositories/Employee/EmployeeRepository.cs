using Dapper;
using FresherMisa2026.Application.Extensions;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Entities.Employee;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
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

        public async Task<(long Total, IEnumerable<Employee> Data)> FilterEmployeesAsync(EmployeeFilterRequest filterRequest)
        {
            var pageSize = filterRequest.PageSize < 1 ? 10 : filterRequest.PageSize;
            var pageIndex = filterRequest.PageIndex < 1 ? 1 : filterRequest.PageIndex;
            var offset = (pageIndex - 1) * pageSize;

            var whereClause = new StringBuilder(" WHERE 1 = 1");
            var parameters = new DynamicParameters();

            if (filterRequest.DepartmentId.HasValue)
            {
                whereClause.Append(" AND DepartmentID = @DepartmentId");
                parameters.Add("@DepartmentId", filterRequest.DepartmentId.Value);
            }

            if (filterRequest.PositionId.HasValue)
            {
                whereClause.Append(" AND PositionID = @PositionId");
                parameters.Add("@PositionId", filterRequest.PositionId.Value);
            }

            if (filterRequest.SalaryFrom.HasValue)
            {
                whereClause.Append(" AND Salary >= @SalaryFrom");
                parameters.Add("@SalaryFrom", filterRequest.SalaryFrom.Value);
            }

            if (filterRequest.SalaryTo.HasValue)
            {
                whereClause.Append(" AND Salary <= @SalaryTo");
                parameters.Add("@SalaryTo", filterRequest.SalaryTo.Value);
            }

            if (filterRequest.Gender.HasValue)
            {
                whereClause.Append(" AND Gender = @Gender");
                parameters.Add("@Gender", filterRequest.Gender.Value);
            }

            if (filterRequest.HireDateFrom.HasValue)
            {
                whereClause.Append(" AND CreatedDate >= @HireDateFrom");
                parameters.Add("@HireDateFrom", filterRequest.HireDateFrom.Value.Date);
            }

            if (filterRequest.HireDateTo.HasValue)
            {
                whereClause.Append(" AND CreatedDate < @HireDateTo");
                parameters.Add("@HireDateTo", filterRequest.HireDateTo.Value.Date.AddDays(1));
            }

            parameters.Add("@Offset", offset);
            parameters.Add("@PageSize", pageSize);

            var query = $@"
SELECT COUNT(*) FROM Employee{whereClause};
SELECT * FROM Employee{whereClause}
ORDER BY EmployeeID DESC
LIMIT @Offset, @PageSize;";

            using var reader = await _dbConnection.QueryMultipleAsync(
                query,
                parameters,
                commandType: CommandType.Text);

            var total = await reader.ReadFirstAsync<long>();
            var data = (await reader.ReadAsync<Employee>()).ToList();

            return (total, data);
        }
    }
}
