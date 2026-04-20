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

        /// <summary>
        /// Lọc nhân viên theo nhiều điều kiện, build query động
        /// </summary>
        public async Task<IEnumerable<Employee>> FilterEmployeesAsync(
            Guid? departmentId,
            Guid? positionId,
            decimal? salaryFrom,
            decimal? salaryTo,
            int? gender,
            DateTime? hireDateFrom,
            DateTime? hireDateTo)
        {
            var query = new StringBuilder("SELECT * FROM Employee WHERE 1=1");
            var parameters = new DynamicParameters();

            if (departmentId.HasValue)
            {
                query.Append(" AND DepartmentID = @DepartmentID");
                parameters.Add("@DepartmentID", departmentId.Value.ToString(), System.Data.DbType.String);
            }

            if (positionId.HasValue)
            {
                query.Append(" AND PositionID = @PositionID");
                parameters.Add("@PositionID", positionId.Value.ToString(), System.Data.DbType.String);
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
                query.Append(" AND CreatedDate >= @HireDateFrom");
                parameters.Add("@HireDateFrom", hireDateFrom.Value);
            }

            if (hireDateTo.HasValue)
            {
                query.Append(" AND CreatedDate <= @HireDateTo");
                parameters.Add("@HireDateTo", hireDateTo.Value);
            }

            return await _dbConnection.QueryAsync<Employee>(
                query.ToString(),
                parameters,
                commandType: System.Data.CommandType.Text);
        }

        /// <summary>
        /// Task 3.3: Lọc nhân viên có phân trang
        /// </summary>
        public async Task<(long Total, IEnumerable<Employee> Data)> FilterEmployeesWithPagingAsync(
            Guid? departmentId,
            Guid? positionId,
            decimal? salaryFrom,
            decimal? salaryTo,
            int? gender,
            DateTime? hireDateFrom,
            DateTime? hireDateTo,
            int pageSize,
            int pageIndex)
        {
            var whereClause = new StringBuilder(" WHERE 1=1");
            var parameters = new DynamicParameters();

            if (departmentId.HasValue)
            {
                whereClause.Append(" AND DepartmentID = @DepartmentID");
                parameters.Add("@DepartmentID", departmentId.Value.ToString(), System.Data.DbType.String);
            }
            if (positionId.HasValue)
            {
                whereClause.Append(" AND PositionID = @PositionID");
                parameters.Add("@PositionID", positionId.Value.ToString(), System.Data.DbType.String);
            }
            if (salaryFrom.HasValue)
            {
                whereClause.Append(" AND Salary >= @SalaryFrom");
                parameters.Add("@SalaryFrom", salaryFrom.Value);
            }
            if (salaryTo.HasValue)
            {
                whereClause.Append(" AND Salary <= @SalaryTo");
                parameters.Add("@SalaryTo", salaryTo.Value);
            }
            if (gender.HasValue)
            {
                whereClause.Append(" AND Gender = @Gender");
                parameters.Add("@Gender", gender.Value);
            }
            if (hireDateFrom.HasValue)
            {
                whereClause.Append(" AND CreatedDate >= @HireDateFrom");
                parameters.Add("@HireDateFrom", hireDateFrom.Value);
            }
            if (hireDateTo.HasValue)
            {
                whereClause.Append(" AND CreatedDate <= @HireDateTo");
                parameters.Add("@HireDateTo", hireDateTo.Value);
            }

            // Paging
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = 10;
            var offset = (pageIndex - 1) * pageSize;

            // Query data + count trong 1 lần gọi
            var dataQuery = $"SELECT * FROM Employee {whereClause} ORDER BY EmployeeCode LIMIT {offset},{pageSize}";
            var countQuery = $"SELECT COUNT(*) FROM Employee {whereClause}";
            var combinedQuery = $"{dataQuery}; {countQuery};";

            using var reader = await _dbConnection.QueryMultipleAsync(
                combinedQuery, parameters, commandType: System.Data.CommandType.Text);

            var data = (await reader.ReadAsync<Employee>()).ToList();
            var total = await reader.ReadFirstAsync<long>();

            return (total, data);
        }
    }
}