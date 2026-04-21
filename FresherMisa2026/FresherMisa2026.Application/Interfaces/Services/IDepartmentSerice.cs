using FresherMisa2026.Entities;
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
        /// <returns></returns>
        /// Created By: dvhai (10/04/2026)
        Task<Department> GetDepartmentByCodeAsync(string code);


        // Lấy danh sách nhân viên theo mã phòng ban
        Task<ServiceResponse> GetEmployeesByDepartmentCodeAsync(string code);
      
        // Lấy số lượng nhân viên theo mã phòng ban
        Task<ServiceResponse> GetEmployeeCountByDepartmentCodeAsync(string code);
    }
}
