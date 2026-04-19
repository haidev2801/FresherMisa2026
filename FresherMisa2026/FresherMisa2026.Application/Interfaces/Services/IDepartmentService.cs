using FresherMisa2026.Entities.Department;
using FresherMisa2026.Entities.Employee;
using System;
using System.Collections.Generic;
using System.Text;

namespace FresherMisa2026.Application.Interfaces.Services
{
    public interface IDepartmentService : IBaseService<Department>
    {
        /// <summary>
        /// Lấy department theo code
        /// </summary>
        /// <returns></returns>
        /// Created By: dvhai (10/04/2026)
        Task<Department> GetDepartmentByCodeAsync(string code);

        /// <summary>
        /// Lấy danh sách nhân viên theo mã phòng ban
        /// </summary>
        /// <param name="code">Mã phòng ban</param>
        /// <returns>Danh sách nhân viên</returns>
        /// Created by: Anhs (20/04/2026)
        Task<IEnumerable<Employee>> GetEmployeesByDepartmentCodeAsync(string code);

        /// <summary>
        /// Đếm số nhân viên trong phòng ban theo mã
        /// </summary>
        /// <param name="code">Mã phòng ban</param>
        /// <returns>Số lượng nhân viên</returns>
        /// Created by: Anhs (20/04/2026)
        Task<long> GetEmployeeCountByDepartmentCodeAsync(string code);
    }
}
