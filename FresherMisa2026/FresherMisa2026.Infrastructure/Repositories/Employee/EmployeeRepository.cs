using Dapper;
using FresherMisa2026.Application.Extensions;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Entities.Employee;
using FresherMisa2026.Entities.Employee.DTO;
using Microsoft.Extensions.Configuration;
using System.Data;
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
            var param = new DynamicParameters();
            param.Add("@p_DepartmentID", departmentId.ToString());

            return await _dbConnection.QueryAsync<Employee>(
                "Proc_Employee_GetByDepartmentId",
                param,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByPositionId(Guid positionId)
        {
            var param = new DynamicParameters();
            param.Add("@p_PositionID", positionId.ToString());

            return await _dbConnection.QueryAsync<Employee>(
                "Proc_Employee_GetByPositionId",
                param,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentCode(string departmentCode)
        {
            var param = new DynamicParameters();
            param.Add("@p_DepartmentCode", departmentCode);

            return await _dbConnection.QueryAsync<Employee>(
                "Proc_Employee_GetByDepartmentCode",
                param,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<int> GetEmployeeCountByDepartmentCode(string departmentCode)
        {
            var param = new DynamicParameters();
            param.Add("@p_DepartmentCode", departmentCode);

            return await _dbConnection.ExecuteScalarAsync<int>(
                "Proc_Employee_GetCountByDepartmentCode",
                param,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Employee>> FilterEmployeesAsync(EmployeeFilterRequest filterRequest)
        {
            var param = new DynamicParameters();
            param.Add("@p_DepartmentID", filterRequest.DepartmentId?.ToString());
            param.Add("@p_PositionID", filterRequest.PositionId?.ToString());
            param.Add("@p_SalaryFrom", filterRequest.SalaryFrom);
            param.Add("@p_SalaryTo", filterRequest.SalaryTo);
            param.Add("@p_Gender", filterRequest.Gender);
            param.Add("@p_HireDateFrom", filterRequest.HireDateFrom?.Date);
            param.Add("@p_HireDateTo", filterRequest.HireDateTo?.Date);

            return await _dbConnection.QueryAsync<Employee>(
                "Proc_Employee_FilterByCondition",
                param,
                commandType: CommandType.StoredProcedure);
        }
    }
}