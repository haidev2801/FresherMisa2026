using Dapper;
using FresherMisa2026.Application.Extensions;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Employee;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Globalization;

namespace FresherMisa2026.Infrastructure.Repositories
{
    public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(IConfiguration configuration, IMemoryCache memoryCache) : base(configuration, memoryCache)
        {
        }

        public async Task<Employee?> GetEmployeeByCode(string code)
        {
            string query = SQLExtension.GetQuery("Employee.GetByCode");
            var param = new Dictionary<string, object>
            {
                {"@EmployeeCode", code }
            };
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            return await connection.QueryFirstOrDefaultAsync<Employee>(query, param, commandType: System.Data.CommandType.Text);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentId(Guid departmentId)
        {
            string query = SQLExtension.GetQuery("Employee.GetByDepartmentId");
            var param = new Dictionary<string, object>
            {
                {"@DepartmentID", departmentId }
            };
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            return await connection.QueryAsync<Employee>(query, param, commandType: System.Data.CommandType.Text);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByPositionId(Guid positionId)
        {
            string query = SQLExtension.GetQuery("Employee.GetByPositionId");
            var param = new Dictionary<string, object>
            {
                {"@PositionID", positionId }
            };
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            return await connection.QueryAsync<Employee>(query, param, commandType: System.Data.CommandType.Text);
        }

        public async Task<PagingResponse<Employee>> GetEmployeesFilterAsync(
            Guid? departmentId,
            Guid? positionId,
            string? salaryFrom,
            string? salaryTo,
            int? gender,
            DateTime? hireDateFrom,
            DateTime? hireDateTo,
            int pageSize,
            int pageIndex)
        {
            if (pageSize < 1)
            {
                pageSize = 10;
            }

            if (pageIndex < 1)
            {
                pageIndex = 1;
            }

            var param = new DynamicParameters();
            var whereClauses = new List<string>();

            if (departmentId.HasValue)
            {
                whereClauses.Add("e.DepartmentID = @DepartmentID");
                param.Add("DepartmentID", departmentId);
            }

            if (positionId.HasValue)
            {
                whereClauses.Add("e.PositionID = @PositionID");
                param.Add("PositionID", positionId);
            }

            if (decimal.TryParse(salaryFrom, NumberStyles.Number, CultureInfo.InvariantCulture, out var salaryFromValue))
            {
                whereClauses.Add("e.Salary >= @SalaryFrom");
                param.Add("SalaryFrom", salaryFromValue);
            }

            if (decimal.TryParse(salaryTo, NumberStyles.Number, CultureInfo.InvariantCulture, out var salaryToValue))
            {
                whereClauses.Add("e.Salary <= @SalaryTo");
                param.Add("SalaryTo", salaryToValue);
            }

            if (gender.HasValue)
            {
                whereClauses.Add("e.Gender = @Gender");
                param.Add("Gender", gender);
            }

            if (hireDateFrom.HasValue)
            {
                whereClauses.Add("DATE(e.HireDate) >= @HireDateFrom");
                param.Add("HireDateFrom", hireDateFrom.Value.Date);
            }

            if (hireDateTo.HasValue)
            {
                whereClauses.Add("DATE(e.HireDate) <= @HireDateTo");
                param.Add("HireDateTo", hireDateTo.Value.Date);
            }

            var whereSql = whereClauses.Count > 0
                ? $"WHERE {string.Join(" AND ", whereClauses)}"
                : string.Empty;

            var offset = (pageIndex - 1) * pageSize;
            param.Add("Offset", offset);
            param.Add("PageSize", pageSize);

            var dataSql = $@"
                SELECT
                    e.EmployeeID,
                    e.EmployeeCode,
                    e.EmployeeName,
                    e.Gender,
                    CASE e.Gender
                        WHEN 0 THEN 'Nữ'
                        WHEN 1 THEN 'Nam'
                        WHEN 2 THEN 'Khác'
                        ELSE 'Không xác định'
                    END AS GenderName,
                    e.DateOfBirth,
                    e.PhoneNumber,
                    e.Email,
                    e.Address,
                    e.DepartmentID,
                    d.DepartmentCode,
                    d.DepartmentName,
                    e.PositionID,
                    p.PositionCode,
                    p.PositionName,
                    e.Salary,
                    e.HireDate,
                    e.CreatedDate
                FROM employee e
                LEFT JOIN department d ON e.DepartmentID = d.DepartmentID
                LEFT JOIN position p ON e.PositionID = p.PositionID
                {whereSql}
                ORDER BY e.CreatedDate DESC
                LIMIT @Offset, @PageSize;";

            var totalSql = $@"
                SELECT COUNT(*) AS Total
                FROM employee e
                {whereSql};";

            await using var connection = CreateConnection();
            await connection.OpenAsync();

            var data = (await connection.QueryAsync<Employee>(dataSql, param, commandType: System.Data.CommandType.Text)).ToList();
            var total = await connection.ExecuteScalarAsync<long>(totalSql, param, commandType: System.Data.CommandType.Text);

            return new PagingResponse<Employee>
            {
                Total = total,
                PageSize = pageSize,
                PageIndex = pageIndex,
                Data = data
            };
        }
    }
}
