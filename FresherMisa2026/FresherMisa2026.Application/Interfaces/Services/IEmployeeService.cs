using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Employee;
using FresherMisa2026.Entities.Employee.DTO;
using System;
using System.Collections.Generic;

namespace FresherMisa2026.Application.Interfaces.Services
{
    /// <summary>
    /// Interface nghiệp vụ nhân viên
    /// </summary>
    public interface IEmployeeService : IBaseService<Employee>
    {
        /// <summary>
        /// Lấy nhân viên theo mã
        /// </summary>
        /// <param name="code">Mã nhân viên</param>
        /// <returns>Nhân viên tìm thấy</returns>
        Task<Employee> GetEmployeeByCodeAsync(string code);

        /// <summary>
        /// Lấy danh sách nhân viên theo phòng ban
        /// </summary>
        /// <param name="departmentId">Id phòng ban</param>
        /// <returns>Danh sách nhân viên</returns>
        Task<IEnumerable<Employee>> GetEmployeesByDepartmentIdAsync(Guid departmentId);

        /// <summary>
        /// Lấy danh sách nhân viên theo vị trí
        /// </summary>
        /// <param name="positionId">Id vị trí</param>
        /// <returns>Danh sách nhân viên</returns>
        Task<IEnumerable<Employee>> GetEmployeesByPositionIdAsync(Guid positionId);

        /// <summary>
        /// Lọc nhân viên theo điều kiện
        /// </summary>
        /// <param name="request">Điều kiện lọc</param>
        /// <returns>Kết quả lọc</returns>
        Task<ServiceResponse> GetFilter(FilterRequest request);
    }
}