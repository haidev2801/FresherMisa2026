using FresherMisa2026.Entities.Employee;
using System;
using System.Collections.Generic;

namespace FresherMisa2026.Application.Interfaces.Repositories
{
    public interface IEmployeeRepository : IBaseRepository<Employee>
    {
        /// <summary>
        /// Lấy employee bằng code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        /// Created by: Phuong (17/04/2026)
        Task<Employee> GetEmployeeByCode(string code);
        Task<IEnumerable<Employee>> GetEmployeesByDepartmentId(Guid departmentId);
        Task<IEnumerable<Employee>> GetEmployeesByPositionId(Guid positionId);

        /// <summary>
        /// Lọc danh sách nhân viên theo nhiều điều kiện
        /// </summary>
        /// Created by: Phuong (18/04/2026)
        Task<(long Total, IEnumerable<Employee> Data)> GetEmployeesFilterAsync(
            Guid? departmentId, 
            Guid? positionId, 
            decimal? salaryFrom, 
            decimal? salaryTo, 
            int? gender, 
            DateTime? hireDateFrom, 
            DateTime? hireDateTo,
            int pageSize,
            int pageIndex);
    }
}
