using FresherMisa2026.Entities.Employee;
using System;
using System.Collections.Generic;

namespace FresherMisa2026.Application.Interfaces.Repositories
{
    public interface IEmployeeRepository : IBaseRepository<Employee>
    {
        Task<Employee> GetEmployeeByEmployeeCodeAsync(string employeeCode);
        Task<IEnumerable<Employee>> GetEmployeesByDepartmentCodeAsync(string departmentCode);
        Task<IEnumerable<Employee>> GetEmployeesByDepartmentId(Guid departmentId);
        Task<IEnumerable<Employee>> GetEmployeesByPositionId(Guid positionId);
        Task<(IEnumerable<Employee> employees, int TotalRecords)> GetFilter(string? departmentId, string? positionId, decimal? salaryFrom, decimal? salaryTo, int? gender, DateTime? hireDateFrom, DateTime? hireDateTo, int PageSize = 20, int PageIndex = 1);
    }
}
