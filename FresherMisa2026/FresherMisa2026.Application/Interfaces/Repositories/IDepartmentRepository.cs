using FresherMisa2026.Entities.Department;
using System;
using System.Collections.Generic;
using System.Text;

namespace FresherMisa2026.Application.Interfaces.Repositories
{
    /// <summary>
    /// Interface truy cập dữ liệu phòng ban
    /// </summary>
    public interface IDepartmentRepository : IBaseRepository<Department>
    {
        /// <summary>
        /// Lấy phòng ban theo mã
        /// </summary>
        /// <param name="code">Mã phòng ban</param>
        /// <returns>Phòng ban tìm thấy</returns>
        Task<Department> GetDepartmentByCodeAsync(string code);
    }
}
