using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Department;
using System;
using System.Collections.Generic;
using System.Text;

namespace FresherMisa2026.Application.Interfaces.Services
{
    /// <summary>
    /// Interface nghiệp vụ phòng ban
    /// </summary>
    public interface IDepartmentSerice : IBaseService<Department>
    {
        /// <summary>
        /// Lấy department theo code
        /// </summary>
        /// <returns></returns>
        /// Created By: dvhai (10/04/2026)
        Task<ServiceResponse> GetDepartmentByCodeAsync(string code);

        /// <summary>
        /// Lấy danh sách nhân viên theo mã phòng ban
        /// </summary>
        /// <param name="code">Mã phòng ban</param>
        /// <returns>Danh sách nhân viên</returns>
        Task<ServiceResponse> GetEmployeesByDepartmentCode(string code);

        /// <summary>
        /// Đếm số nhân viên theo mã phòng ban
        /// </summary>
        /// <param name="code">Mã phòng ban</param>
        /// <returns>Số lượng nhân viên</returns>
        Task<ServiceResponse> GetEmployeeCountByDepartmentCode(string code);
    }
}
