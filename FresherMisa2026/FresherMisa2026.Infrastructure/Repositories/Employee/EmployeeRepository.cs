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

        public async Task<(long Total, IEnumerable<Employee> Data)> FilterEmployees(
            Guid? departmentId,
            Guid? positionId,
            decimal? salaryFrom,
            decimal? salaryTo,
            int? gender,
            DateTime? hireDateFrom,
            DateTime? hireDateTo,
            int pageSize = 10,
            int pageIndex = 1)
        {
            // Xây dựng truy vấn SQL động
            var sql = @"SELECT * FROM Employee WHERE 1=1";
            var countSql = @"SELECT COUNT(*) FROM Employee WHERE 1=1";
            var param = new DynamicParameters();

            if (departmentId.HasValue && departmentId.Value != Guid.Empty)
            {
                sql += " AND DepartmentID = @DepartmentID";
                countSql += " AND DepartmentID = @DepartmentID";
                param.Add("DepartmentID", departmentId);
            }
            if (positionId.HasValue && positionId.Value != Guid.Empty)
            {
                sql += " AND PositionID = @PositionID";
                countSql += " AND PositionID = @PositionID";
                param.Add("PositionID", positionId);
            }
            if (salaryFrom.HasValue)
            {
                sql += " AND Salary >= @SalaryFrom";
                countSql += " AND Salary >= @SalaryFrom";
                param.Add("SalaryFrom", salaryFrom);
            }
            if (salaryTo.HasValue)
            {
                sql += " AND Salary <= @SalaryTo";
                countSql += " AND Salary <= @SalaryTo";
                param.Add("SalaryTo", salaryTo);
            }
            if (gender.HasValue)
            {
                sql += " AND Gender = @Gender";
                countSql += " AND Gender = @Gender";
                param.Add("Gender", gender);
            }
            if (hireDateFrom.HasValue)
            {
                sql += " AND HireDate >= @HireDateFrom";
                countSql += " AND HireDate >= @HireDateFrom";
                param.Add("HireDateFrom", hireDateFrom);
            }
            if (hireDateTo.HasValue)
            {
                sql += " AND HireDate <= @HireDateTo";
                countSql += " AND HireDate <= @HireDateTo";
                param.Add("HireDateTo", hireDateTo);
            }

            // Thêm LIMIT/OFFSET để phân trang
            // pageIndex bắt đầu từ 1, OFFSET = (pageIndex - 1) * pageSize
            int offset = (pageIndex - 1) * pageSize;
            sql += " LIMIT @PageSize OFFSET @Offset";
            param.Add("PageSize", pageSize);
            param.Add("Offset", offset);

            var data = await _dbConnection.QueryAsync<Employee>(sql, param, commandType: System.Data.CommandType.Text);
            var total = await _dbConnection.ExecuteScalarAsync<long>(countSql, param, commandType: System.Data.CommandType.Text);
            return (total, data);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentCode(string departmentCode)
        {
            string query = @"SELECT e.* FROM Employee e
                             INNER JOIN Department d ON e.DepartmentID = d.DepartmentID
                             WHERE d.DepartmentCode = @DepartmentCode";
            var param = new { DepartmentCode = departmentCode };
            return await _dbConnection.QueryAsync<Employee>(query, param, commandType: System.Data.CommandType.Text);
        }

        public async Task<int> CountEmployeesByDepartmentCode(string departmentCode)
        {
            string query = @"SELECT COUNT(*) FROM Employee e
                             INNER JOIN Department d ON e.DepartmentID = d.DepartmentID
                             WHERE d.DepartmentCode = @DepartmentCode";
            var param = new { DepartmentCode = departmentCode };
            return await _dbConnection.ExecuteScalarAsync<int>(query, param, commandType: System.Data.CommandType.Text);
        }
    }
}