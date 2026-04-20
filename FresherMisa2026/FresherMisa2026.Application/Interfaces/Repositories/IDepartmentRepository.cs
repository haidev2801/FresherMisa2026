using FresherMisa2026.Entities.Department;
using FresherMisa2026.Entities.Employee;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
        /// Created By: Phuong (18/04/2026)
        Task<IEnumerable<Employee>> GetEmployeesByDepartmentCode(string code);

        /// <summary>
        /// Đếm số lượng nhân viên trong phòng ban theo mã
        /// </summary>
        /// <param name="code">Mã phòng ban</param>
        /// Created By: Phuong (18/04/2026)
        Task<int> GetEmployeeCountByDepartmentCode(string code);
    }
}
