using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Employee;
using System;
using System.Collections.Generic;

namespace FresherMisa2026.Application.Interfaces.Services
{
    public interface IEmployeeService : IBaseService<Employee>
    {
        Task<Employee?> GetEmployeeByCodeAsync(string code);
        Task<IEnumerable<Employee>> GetEmployeesByDepartmentIdAsync(Guid departmentId);
        Task<IEnumerable<Employee>> GetEmployeesByPositionIdAsync(Guid positionId);
        Task<PagingResponse<Employee>> GetEmployeesFilterAsync(
            Guid? departmentId,
            Guid? positionId,
            string? salaryFrom,
            string? salaryTo,
            int? gender,
            DateTime? hireDateFrom,
            DateTime? hireDateTo,
            int pageSize,
            int pageIndex);
    }
}
