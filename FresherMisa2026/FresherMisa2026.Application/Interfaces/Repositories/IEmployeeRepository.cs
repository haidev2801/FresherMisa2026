using FresherMisa2026.Entities.Employee;
using System;
using System.Collections.Generic;

namespace FresherMisa2026.Application.Interfaces.Repositories
{
    public interface IEmployeeRepository : IBaseRepository<Employee>
    {
        Task<Employee> GetEmployeeByCode(string code);
        Task<IEnumerable<Employee>> GetEmployeesByDepartmentId(Guid departmentId);
        Task<IEnumerable<Employee>> GetEmployeesByPositionId(Guid positionId);

        /// <summary>
        /// Lọc nhân viên theo nhiều điều kiện
        /// </summary>
        Task<IEnumerable<Employee>> FilterEmployeesAsync(
            Guid? departmentId,
            Guid? positionId,
            decimal? salaryFrom,
            decimal? salaryTo,
            int? gender,
            DateTime? hireDateFrom,
            DateTime? hireDateTo);

        /// <summary>
        /// Task 3.3: Lọc nhân viên có phân trang
        /// </summary>
        Task<(long Total, IEnumerable<Employee> Data)> FilterEmployeesWithPagingAsync(
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
