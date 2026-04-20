using FresherMisa2026.Entities.Department;
using FresherMisa2026.Entities.Employee;
using System;
using System.Collections.Generic;
using System.Text;

namespace FresherMisa2026.Application.Interfaces.Services
{
    public interface IDepartmentSerice : IBaseService<Department>
    {
        /// <summary>
        /// Lấy department theo code
        /// </summary>
        Task<Department> GetDepartmentByCodeAsync(string code);

        /// <summary>
        /// Task 2.3: Lấy danh sách nhân viên theo mã phòng ban
        /// </summary>
        Task<IEnumerable<Employee>> GetEmployeesByDepartmentCodeAsync(string code);

        /// <summary>
        /// Task 2.3: Đếm số nhân viên theo mã phòng ban
        /// </summary>
        Task<int> GetEmployeeCountByDepartmentCodeAsync(string code);
    }
}
