using Dapper;
using FresherMisa2026.Application.Extensions;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Entities.Employee;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace FresherMisa2026.Infrastructure.Repositories
{
    public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(IConfiguration configuration, IMemoryCache memoryCache) : base(configuration, memoryCache)
        {
        }

        public async Task<Employee> GetEmployeeByCode(string code)
        {
            string query = SQLExtension.GetQuery("Employee.GetByCode");
            var param = new Dictionary<string, object>
            {
                {"@EmployeeCode", code }
            };
            using (var dbConnection = await OpenConnectionAsync())
            {
                return await dbConnection.QueryFirstOrDefaultAsync<Employee>(query, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentId(Guid departmentId)
        {
            string query = SQLExtension.GetQuery("Employee.GetByDepartmentId");
            var param = new Dictionary<string, object>
            {
                {"@DepartmentID", departmentId }
            };
            using (var dbConnection = await OpenConnectionAsync())
            {
                return await dbConnection.QueryAsync<Employee>(query, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByPositionId(Guid positionId)
        {
            string query = SQLExtension.GetQuery("Employee.GetByPositionId");
            var param = new Dictionary<string, object>
            {
                {"@PositionID", positionId }
            };
            using (var dbConnection = await OpenConnectionAsync())
            {
                return await dbConnection.QueryAsync<Employee>(query, param, commandType: CommandType.Text);
            }
        }

        /// <summary>
        /// Lọc và phân trang danh sách nhân viên theo các tiêu chí: 
        /// phòng ban, chức vụ, mức lương, giới tính, ngày tuyển dụng
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="departmentId"></param>
        /// <param name="positionId"></param>
        /// <param name="salaryFrom"></param>
        /// <param name="salaryTo"></param>
        /// <param name="gender"></param>
        /// <param name="hireDateFrom"></param>
        /// <param name="hireDateTo"></param>
        /// <returns></returns>
        /// Created By: Tannn (18/04/2026)
        public async Task<(long Total, IEnumerable<Employee> Employees)> GetFilterEmployeesAsync(
            int pageSize,
            int pageIndex,
            Guid? departmentId,
            Guid? positionId,
            decimal? salaryFrom,
            decimal? salaryTo,
            int? gender,
            DateTime? hireDateFrom,
            DateTime? hireDateTo)
        {
            var param = new DynamicParameters();

            param.Add("p_DepartmentId", departmentId);
            param.Add("p_PositionId", positionId);
            param.Add("p_SalaryFrom", salaryFrom);
            param.Add("p_SalaryTo", salaryTo);
            param.Add("p_Gender", gender);
            param.Add("p_HireDateFrom", hireDateFrom);
            param.Add("p_HireDateTo", hireDateTo);
            param.Add("p_PageSize", pageSize);
            param.Add("p_PageIndex", pageIndex);

            using (var dbConnection = await OpenConnectionAsync())
            {
                using var multi = await dbConnection.QueryMultipleAsync(
                    "Proc_FilterEmployees",
                    param,
                    commandType: CommandType.StoredProcedure
                );

                var total = await multi.ReadFirstAsync<long>();
                var employees = await multi.ReadAsync<Employee>();

                return (total, employees);
            }
        }
    }
}