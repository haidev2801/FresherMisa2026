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

        public async Task<(long Total, IEnumerable<Employee> Data)> FilterEmployees(Guid? departmentId, Guid? positionId, decimal? salaryFrom, decimal? salaryTo, int? gender, DateTime? hireDateFrom, DateTime? hireDateTo, int pageSize, int pageIndex)
        {
            // Build dynamic query
            var where = new StringBuilder();

            var parameters = new DynamicParameters();

            if (departmentId.HasValue && departmentId != Guid.Empty)
            {
                where.Append(" AND DepartmentID = @DepartmentID");
                parameters.Add("@DepartmentID", departmentId.Value);
            }

            if (positionId.HasValue && positionId != Guid.Empty)
            {
                where.Append(" AND PositionID = @PositionID");
                parameters.Add("@PositionID", positionId.Value);
            }

            if (salaryFrom.HasValue)
            {
                where.Append(" AND Salary >= @SalaryFrom");
                parameters.Add("@SalaryFrom", salaryFrom.Value);
            }

            if (salaryTo.HasValue)
            {
                where.Append(" AND Salary <= @SalaryTo");
                parameters.Add("@SalaryTo", salaryTo.Value);
            }

            if (gender.HasValue)
            {
                where.Append(" AND Gender = @Gender");
                parameters.Add("@Gender", gender.Value);
            }

            // Use CreatedDate as hire date if provided
            if (hireDateFrom.HasValue)
            {
                where.Append(" AND HireDate >= @HireDateFrom");
                parameters.Add("@HireDateFrom", hireDateFrom.Value.Date);
            }

            if (hireDateTo.HasValue)
            {
                where.Append(" AND HireDate <= @HireDateTo");
                parameters.Add("@HireDateTo", hireDateTo.Value.Date);
            }

            // Prepare count query
            //var countSql = new StringBuilder("SELECT COUNT(1) FROM Employee WHERE 1=1");
            var countSql = new StringBuilder(SQLExtension.GetQuery("Employee.Count"));
            countSql.Append(where.ToString());

            // Ensure pageIndex and pageSize have sensible values
            if (pageSize <= 0) pageSize = 10;
            if (pageIndex <= 0) pageIndex = 1;

            var offset = (pageIndex - 1) * pageSize;

            // Prepare data query with limit/offset
            //var dataSql = new StringBuilder("SELECT * FROM Employee WHERE 1=1");
            var dataSql = new StringBuilder(SQLExtension.GetQuery("Employee.FilterBase"));
            dataSql.Append(where.ToString());
            dataSql.Append(" LIMIT @PageSize OFFSET @Offset");

            parameters.Add("@PageSize", pageSize);
            parameters.Add("@Offset", offset);

            using var connection = await OpenConnectionAsync();

            var total = await connection.ExecuteScalarAsync<long>(countSql.ToString(), parameters, commandType: System.Data.CommandType.Text);
            var data = await connection.QueryAsync<Employee>(dataSql.ToString(), parameters, commandType: System.Data.CommandType.Text);

            return (total, data);
        }
    }
}