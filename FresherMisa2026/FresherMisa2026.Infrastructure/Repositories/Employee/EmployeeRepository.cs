using Dapper;
using FresherMisa2026.Application.Dtos.Employee;
using FresherMisa2026.Application.Extensions;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Entities.Employee;
using Microsoft.Extensions.Configuration;
using System.Data;
using static Dapper.SqlMapper;

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

        public async Task<(long Total, IEnumerable<EmployeeDto> Data)>
            GetEmployeesFilter(GetEmployeeFilterDto dto)
        {
            string storedProcedureName = "Proc_Employee_Filter";

            var parameters = new DynamicParameters();
            parameters.Add("@p_PageIndex", dto.PageIndex);
            parameters.Add("@p_PageSize", dto.PageSize);
            parameters.Add("@p_DepartmentId", dto.DepartmentId);
            parameters.Add("@p_PositionId", dto.PositionId);
            parameters.Add("@p_SalaryFrom", dto.SalaryFrom);
            parameters.Add("@p_SalaryTo", dto.SalaryTo);
            parameters.Add("@p_Gender", dto.Gender);
            parameters.Add("@p_HireDateFrom", dto.HireDateFrom);
            parameters.Add("@p_HireDateTo", dto.HireDateTo);

            using var reader = await _dbConnection.QueryMultipleAsync(
                storedProcedureName,
                parameters,
                commandType: CommandType.StoredProcedure);

            List<EmployeeDto> data = (await reader.ReadAsync<EmployeeDto>()).ToList();
            var total = await reader.ReadFirstAsync<long>();

            return (total, data);
        }
    }
}