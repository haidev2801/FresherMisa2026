using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Employee;
using FresherMisa2026.Entities.Employee.DTO;
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
        /// Lọc và phân trang danh sách nhân viên 
        /// </summary>
        /// <param name="filterRequest"></param>
        /// <returns></returns>
        /// Created by: Tannn (18/04/2026)
        Task<ServiceResponse> GetFilterEmployeesAsync(FilterRequest filterRequest);

    }
}