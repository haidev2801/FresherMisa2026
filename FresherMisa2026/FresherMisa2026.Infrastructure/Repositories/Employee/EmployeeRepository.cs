using Dapper;
using FresherMisa2026.Application.Exceptions;
using FresherMisa2026.Application.Extensions;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Entities.Employee;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System.Collections.Generic;

namespace FresherMisa2026.Infrastructure.Repositories
{
    public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(IConfiguration configuration, IMemoryCache cache)
            : base(configuration, cache)
        {
        }

        /// <summary>
        /// MySQL duplicate key (1062) hoặc thông báo trùng mã từ procedure — map sang <see cref="DuplicateEmployeeCodeException"/> cho tầng service.
        /// </summary>
        private static bool IsEmployeeCodeDuplicateFromDb(MySqlException ex)
        {
            if (ex.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
                return true;
            var m = ex.Message;
            if (m.Contains("UK_employee_EmployeeCode", StringComparison.OrdinalIgnoreCase))
                return true;
            if (m.Contains("EmployeeCode", StringComparison.OrdinalIgnoreCase) && m.Contains("Duplicate", StringComparison.OrdinalIgnoreCase))
                return true;
            if (m.Contains("EmployeeCode đã tồn tại", StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }

        public override async Task<int> InsertAsync(Employee entity)
        {
            try
            {
                return await base.InsertAsync(entity);
            }
            catch (MySqlException ex) when (IsEmployeeCodeDuplicateFromDb(ex))
            {
                throw new DuplicateEmployeeCodeException();
            }
        }

        public override async Task<int> UpdateAsync(Guid entityId, Employee entity)
        {
            try
            {
                return await base.UpdateAsync(entityId, entity);
            }
            catch (MySqlException ex) when (IsEmployeeCodeDuplicateFromDb(ex))
            {
                throw new DuplicateEmployeeCodeException();
            }
        }

        public async Task<Employee> GetEmployeeByCode(string code)
        {
            string query = SQLExtension.GetQuery("Employee.GetByCode");
            var param = new Dictionary<string, object>
            {
                {"@EmployeeCode", code }
            };
            await using var connection = await CreateAndOpenConnectionAsync();
            return await connection.QueryFirstOrDefaultAsync<Employee>(query, param, commandType: System.Data.CommandType.Text);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentId(Guid departmentId)
        {
            string query = SQLExtension.GetQuery("Employee.GetByDepartmentId");
            var param = new Dictionary<string, object>
            {
                {"@DepartmentID", departmentId }
            };
            await using var connection = await CreateAndOpenConnectionAsync();
            return await connection.QueryAsync<Employee>(query, param, commandType: System.Data.CommandType.Text);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByPositionId(Guid positionId)
        {
            string query = SQLExtension.GetQuery("Employee.GetByPositionId");
            var param = new Dictionary<string, object>
            {
                {"@PositionID", positionId }
            };
            await using var connection = await CreateAndOpenConnectionAsync();
            return await connection.QueryAsync<Employee>(query, param, commandType: System.Data.CommandType.Text);
        }

        /// <summary>
        /// Lọc nhân viên có phân trang: tổng bản ghi khớp điều kiện + dữ liệu trang hiện tại.
        /// </summary>
        public async Task<(long Total, IEnumerable<Employee> Data)> GetEmployeesByFilterAsync(EmployeeFilterRequest filter)
        {
            var param = new Dictionary<string, object>
            {
                { "@DepartmentID", filter.DepartmentID },
                { "@PositionID", filter.PositionID },
                { "@salaryFrom", filter.SalaryFrom },
                { "@salaryTo", filter.SalaryTo },
                { "@gender", filter.Gender },
                { "@hireDateFrom", filter.HireDateFrom },
                { "@hireDateTo", filter.HireDateTo },
            };

            var pageSize = filter.PageSize;
            var offset = (filter.PageIndex - 1) * pageSize;
            param["@pageSize"] = pageSize;
            param["@offset"] = offset;

            await using var connection = await CreateAndOpenConnectionAsync();
            var countSql = SQLExtension.GetQuery("Employee.GetByFilterCount");
            var total = await connection.ExecuteScalarAsync<long>(countSql, param, commandType: System.Data.CommandType.Text);

            var dataSql = SQLExtension.GetQuery("Employee.GetByFilter");
            var rows = await connection.QueryAsync<Employee>(dataSql, param, commandType: System.Data.CommandType.Text);

            return (total, rows.ToList());
        }
    }
}