using FresherMisa2026.Entities.Employee;
using System;
using System.Collections.Generic;
using System.Text;

namespace FresherMisa2026.Application.Interfaces.Services
{
    public interface IEmployeeService : IBaseService<Employee>
    {
        /// <summary>
        /// Lấy employee theo code
        /// </summary>
        /// <param name="code">Mã employee</param>
        /// <returns>Employee tìm thấy</returns>
        Task<Employee> GetEmployeeByCodeAsync(string code);

        /// <summary>
        /// Lấy danh sách employee theo department id
        /// </summary>
        /// <param name="departmentId">ID phòng ban</param>
        /// <returns>Danh sách employee</returns>
        Task<IEnumerable<Employee>> GetEmployeesByDepartmentIdAsync(Guid departmentId);

        /// <summary>
        /// Lấy danh sách employee theo position id
        /// </summary>
        /// <param name="positionId">ID vị trí</param>
        /// <returns>Danh sách employee</returns>
        Task<IEnumerable<Employee>> GetEmployeesByPositionIdAsync(Guid positionId);
    }
}