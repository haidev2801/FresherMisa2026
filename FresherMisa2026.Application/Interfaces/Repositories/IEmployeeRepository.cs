using FresherMisa2026.Entities.Employee;
using System;
using System.Collections.Generic;
using System.Text;

namespace FresherMisa2026.Application.Interfaces.Repositories
{
    public interface IEmployeeRepository : IBaseRepository<Employee>
    {
        /// <summary>
        /// Lấy employee theo code
        /// </summary>
        /// <param name="code">Mã employee</param>
        /// <returns>Employee tìm thấy hoặc null</returns>
        Task<Employee> GetEmployeeByCode(string code);

        /// <summary>
        /// Lấy danh sách employee theo department id
        /// </summary>
        /// <param name="departmentId">ID phòng ban</param>
        /// <returns>Danh sách employee</returns>
        Task<IEnumerable<Employee>> GetEmployeesByDepartmentId(Guid departmentId);

        /// <summary>
        /// Lấy danh sách employee theo position id
        /// </summary>
        /// <param name="positionId">ID vị trí</param>
        /// <returns>Danh sách employee</returns>
        Task<IEnumerable<Employee>> GetEmployeesByPositionId(Guid positionId);
    }
}