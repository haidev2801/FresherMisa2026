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
        /// Created By: dvhai (10/04/2026)
        Task<Department> GetDepartmentByCodeAsync(string code);

        /// <summary>
        /// Lấy danh sách nhân viên theo mã phòng ban
        /// </summary>
        Task<IEnumerable<Employee>> GetEmployeesByDepartmentCodeAsync(string code);

        /// <summary>
        /// Đếm số nhân viên trong phòng ban theo mã phòng ban
        /// </summary>
        Task<int> GetEmployeeCountByDepartmentCodeAsync(string code);
    }
}
