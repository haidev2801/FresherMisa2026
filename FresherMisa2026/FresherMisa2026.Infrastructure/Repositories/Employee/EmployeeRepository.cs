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
        public EmployeeRepository(
            IConfiguration configuration,
            IMemoryCache memoryCache) : base(configuration, memoryCache)
        {
        }

        public async Task<Employee> GetEmployeeByCode(string code)
        {
            using var connection = CreateConnection();
            string query = SQLExtension.GetQuery("Employee.GetByCode");
            var param = new Dictionary<string, object>
            {
                {"@EmployeeCode", code }
            };
            return await connection.QueryFirstOrDefaultAsync<Employee>(query, param, commandType: System.Data.CommandType.Text);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentId(Guid departmentId)
        {
            using var connection = CreateConnection();
            string query = SQLExtension.GetQuery("Employee.GetByDepartmentId");
            var param = new Dictionary<string, object>
            {
                {"@DepartmentID", departmentId }
            };
            return await connection.QueryAsync<Employee>(query, param, commandType: System.Data.CommandType.Text);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByPositionId(Guid positionId)
        {
            using var connection = CreateConnection();
            string query = SQLExtension.GetQuery("Employee.GetByPositionId");
            var param = new Dictionary<string, object>
            {
                {"@PositionID", positionId }
            };
            return await connection.QueryAsync<Employee>(query, param, commandType: System.Data.CommandType.Text);
        }

        public async Task<IEnumerable<Employee>> FilterEmployees(EmployeeFilterRequest request)
        {
            using var connection = CreateConnection();
            string query = SQLExtension.GetQuery("Employee.Filter");
            var param = new DynamicParameters();

            param.Add("@DepartmentID", request.DepartmentId);
            param.Add("@PositionID", request.PositionId);
            param.Add("@SalaryFrom", request.SalaryFrom);
            param.Add("@SalaryTo", request.SalaryTo);
            param.Add("@Gender", request.Gender);
            param.Add("@HireDateFrom", request.HireDateFrom?.Date);
            param.Add("@HireDateTo", request.HireDateTo?.Date);

            return await connection.QueryAsync<Employee>(query, param, commandType: System.Data.CommandType.Text);
        }
    }
}
