using FresherMisa2026.Entities.Employee;
using System;
using System.Collections.Generic;

namespace FresherMisa2026.Application.Interfaces.Services
{
    public interface IEmployeeService : IBaseService<Employee>
    {
        Task<Employee> GetEmployeeByCodeAsync(string code);
        Task<IEnumerable<Employee>> GetEmployeesByDepartmentIdAsync(Guid departmentId);
        Task<IEnumerable<Employee>> GetEmployeesByPositionIdAsync(Guid positionId);

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
        Task<Entities.PagingResponse<Employee>> FilterEmployeesWithPagingAsync(
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