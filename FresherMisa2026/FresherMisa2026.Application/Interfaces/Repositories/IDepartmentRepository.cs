using FresherMisa2026.Entities.Department;
using FresherMisa2026.Entities.Employee;
using System;
using System.Collections.Generic;
using System.Text;

namespace FresherMisa2026.Application.Interfaces.Repositories
{
    public interface IDepartmentRepository : IBaseRepository<Department>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        Task<Department> GetDepartmentByCode(string code);
        /// <summary>
        /// Lấy danh sách nhân viên theo mã phòng ban
        /// </summary>
        /// <param name="code">Mã phòng ban</param>
        Task<IEnumerable<Employee>> GetEmployeesByDepartmentCode(string departmentCode);
        /// <summary>
        /// Lấy số lượng nhân viên theo mã phòng ban
        /// </summary>
        /// <param name="departmentCode">Mã phòng ban</param>
        Task<int> GetCountEmployeesByDepartmentCode(string departmentCode);
    }
}
