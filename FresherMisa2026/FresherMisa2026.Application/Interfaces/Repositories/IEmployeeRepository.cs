using FresherMisa2026.Entities.Employee;
using System;
using System.Collections.Generic;

namespace FresherMisa2026.Application.Interfaces.Repositories
{
    /// <summary>
    /// Interface truy cập dữ liệu nhân viên
    /// </summary>
    public interface IEmployeeRepository : IBaseRepository<Employee>
    {
        /// <summary>
        /// Lấy nhân viên theo mã
        /// </summary>
        /// <param name="employeeCode">Mã nhân viên</param>
        /// <returns>Nhân viên tìm thấy</returns>
        Task<Employee> GetEmployeeByEmployeeCodeAsync(string employeeCode);

        /// <summary>
        /// Lấy nhân viên theo mã phòng ban
        /// </summary>
        /// <param name="departmentCode">Mã phòng ban</param>
        /// <returns>Danh sách nhân viên</returns>
        Task<IEnumerable<Employee>> GetEmployeesByDepartmentCodeAsync(string departmentCode);

        /// <summary>
        /// Lấy nhân viên theo Id phòng ban
        /// </summary>
        /// <param name="departmentId">Id phòng ban</param>
        /// <returns>Danh sách nhân viên</returns>
        Task<IEnumerable<Employee>> GetEmployeesByDepartmentId(Guid departmentId);

        /// <summary>
        /// Lấy nhân viên theo Id vị trí
        /// </summary>
        /// <param name="positionId">Id vị trí</param>
        /// <returns>Danh sách nhân viên</returns>
        Task<IEnumerable<Employee>> GetEmployeesByPositionId(Guid positionId);

        /// <summary>
        /// Lọc danh sách nhân viên theo nhiều điều kiện
        /// </summary>
        /// <returns>Danh sách nhân viên và tổng số bản ghi</returns>
        Task<(IEnumerable<Employee> employees, int TotalRecords)> GetFilter(string? departmentId, string? positionId, decimal? salaryFrom, decimal? salaryTo, int? gender, DateTime? hireDateFrom, DateTime? hireDateTo, int PageSize = 20, int PageIndex = 1);
    }
}
