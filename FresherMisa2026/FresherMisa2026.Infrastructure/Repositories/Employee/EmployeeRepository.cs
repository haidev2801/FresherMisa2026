using Dapper;
using FresherMisa2026.Application.Extensions;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Entities.Employee;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

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

        public async Task<(long Total, IEnumerable<Employee> Data)> FilterEmployees(
            Guid? departmentId,
            Guid? positionId,
            decimal? salaryFrom,
            decimal? salaryTo,
            int? gender,
            DateTime? hireDateFrom,
            DateTime? hireDateTo,
            int pageSize = 20,
            int pageIndex = 1)
        {
            var whereClause = " WHERE 1=1";
            var param = new DynamicParameters();

            if (departmentId.HasValue)
            {
                whereClause += " AND DepartmentID = @DepartmentID";
                param.Add("@DepartmentID", departmentId.Value.ToString());
            }

            if (positionId.HasValue)
            {
                whereClause += " AND PositionID = @PositionID";
                param.Add("@PositionID", positionId.Value.ToString());
            }

            if (salaryFrom.HasValue)
            {
                whereClause += " AND Salary >= @SalaryFrom";
                param.Add("@SalaryFrom", salaryFrom.Value);
            }

            if (salaryTo.HasValue)
            {
                whereClause += " AND Salary <= @SalaryTo";
                param.Add("@SalaryTo", salaryTo.Value);
            }

            if (gender.HasValue)
            {
                whereClause += " AND Gender = @Gender";
                param.Add("@Gender", gender.Value);
            }

            if (hireDateFrom.HasValue)
            {
                whereClause += " AND HireDateFrom >= @HireDateFrom";
                param.Add("@HireDateFrom", hireDateFrom.Value);
            }

            if (hireDateTo.HasValue)
            {
                whereClause += " AND HireDateTo <= @HireDateTo";
                param.Add("@HireDateTo", hireDateTo.Value);
            }

            // Đếm tổng số bản ghi
            var countSql = $"SELECT COUNT(*) FROM Employee{whereClause}";
            long total = await _dbConnection.ExecuteScalarAsync<long>(countSql, param, commandType: System.Data.CommandType.Text);

            // Phân trang với LIMIT OFFSET
            int offset = (pageIndex - 1) * pageSize;
            param.Add("@PageSize", pageSize);
            param.Add("@Offset", offset);

            var dataSql = $"SELECT * FROM Employee{whereClause} ORDER BY EmployeeCode ASC LIMIT @PageSize OFFSET @Offset";
            var data = await _dbConnection.QueryAsync<Employee>(dataSql, param, commandType: System.Data.CommandType.Text);

            return (total, data);
        }
    }
}