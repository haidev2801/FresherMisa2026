using FresherMisa2026.Entities;
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
        /// Lọc nhân viên theo nhiều điều kiện (có phân trang)
        /// </summary>
        Task<PagingResponse<Employee>> FilterEmployeesAsync(
            Guid? departmentId,
            Guid? positionId,
            decimal? salaryFrom,
            decimal? salaryTo,
            int? gender,
            DateTime? hireDateFrom,
            DateTime? hireDateTo,
            int pageSize = 20,
            int pageIndex = 1);
    }
}