using FresherMisa2026.Entities.Department;
using FresherMisa2026.Entities.Employee;
using System;
using System.Collections.Generic;
using System.Text;

namespace FresherMisa2026.Application.Interfaces.Repositories
{
    public interface IDepartmentRepository : IBaseRepository<Department>
    {
        Task<Department> GetDepartmentByCode(string code);

        /// <summary>
        /// Task 2.3: Lấy danh sách nhân viên theo mã phòng ban
        /// </summary>
        Task<IEnumerable<Employee>> GetEmployeesByDepartmentCode(string code);

        /// <summary>
        /// Task 2.3: Đếm số nhân viên trong phòng ban theo mã
        /// </summary>
        Task<int> GetEmployeeCountByDepartmentCode(string code);
    }
}
