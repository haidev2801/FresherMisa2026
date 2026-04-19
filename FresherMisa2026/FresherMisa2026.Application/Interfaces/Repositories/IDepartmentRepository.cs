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
        /// Lấy danh sách nhân viên theo department code
        /// </summary>
        /// <param name="departmentCode"></param>
        /// <returns></returns>
        /// Created By: tannn (18/04/2026)
        Task<IEnumerable<Employee>> GetEmployeesByDepartmentCode(string departmentCode);

        /// <summary>
        /// Lấy số lượng nhân viên theo department code
        /// </summary>
        /// <param name="departmentCode"></param>
        /// <returns></returns>
        /// Created By: tannn (18/04/2026)
        Task<int> CountEmployeesByDepartmentCode(string departmentCode);
    }
}
