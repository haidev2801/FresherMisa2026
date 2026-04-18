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

        /// <summary>
        /// Lấy employee bằng code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        /// Created by: Phuong (17/04/2026)
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
        /// Lọc nhân viên theo điều kiện
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="positionId"></param>
        /// <param name="salaryFrom"></param>
        /// <param name="salaryTo"></param>
        /// <param name="gender"></param>
        /// <param name="hireDateFrom"></param>
        /// <param name="hireDateTo"></param>
        /// <returns></returns>
        /// Created by: Phuong (18/04/2026)
        public async Task<IEnumerable<Employee>> GetEmployeesFilterAsync(
            Guid? departmentId, 
            Guid? positionId, 
            decimal? salaryFrom, 
            decimal? salaryTo, 
            int? gender, 
            DateTime? hireDateFrom, 
            DateTime? hireDateTo)
        {
            var sql = new StringBuilder($"SELECT * FROM {_tableName} WHERE 1=1 ");
            var parameters = new DynamicParameters();

            if (departmentId.HasValue) { sql.Append(" AND DepartmentID = @DepartmentID"); parameters.Add("DepartmentID", departmentId); }
            if (positionId.HasValue) { sql.Append(" AND PositionID = @PositionID"); parameters.Add("PositionID", positionId); }
            if (gender.HasValue) { sql.Append(" AND Gender = @Gender"); parameters.Add("Gender", gender); }
            
            if (salaryFrom.HasValue) { sql.Append(" AND Salary >= @SalaryFrom"); parameters.Add("SalaryFrom", salaryFrom); }
            if (salaryTo.HasValue) { sql.Append(" AND Salary <= @SalaryTo"); parameters.Add("SalaryTo", salaryTo); }

            if (hireDateFrom.HasValue) { sql.Append(" AND HireDate >= @HireDateFrom"); parameters.Add("HireDateFrom", hireDateFrom); }
            if (hireDateTo.HasValue) { sql.Append(" AND HireDate <= @HireDateTo"); parameters.Add("HireDateTo", hireDateTo); }

            // Đảm bảo không lấy các bản ghi đã xóa nếu có cột IsDeleted
            if (typeof(Employee).GetProperty("IsDeleted") != null)
            {
                sql.Append(" AND IsDeleted = FALSE");
            }

            sql.Append(" ORDER BY CreatedDate DESC");

            return await _dbConnection.QueryAsync<Employee>(sql.ToString(), parameters, commandType: System.Data.CommandType.Text);
        }
    }
}