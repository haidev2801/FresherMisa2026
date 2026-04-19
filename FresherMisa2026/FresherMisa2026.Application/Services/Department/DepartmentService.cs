using FresherMisa2026.Application.Interfaces;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Application.Interfaces.Services;
using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Department;
using FresherMisa2026.Entities.Employee;
using System;
using System.Collections.Generic;
using System.Text;

namespace FresherMisa2026.Application.Services
{
    public class DepartmentService : BaseService<Department>, IDepartmentService
    {
        private readonly IDepartmentRepository _deptRepository;

        public DepartmentService(
            IBaseRepository<Department> baseRepository,
            IDepartmentRepository departmentRepository
            ) : base(baseRepository)
        {
            _deptRepository = departmentRepository;
        }

        /// <summary>
        /// Lấy department theo code
        /// </summary>
        /// <returns></returns>
        /// Created By: dvhai (10/04/2026)
        /// Updated by: Anhs (20/04/2026) - Thêm validate mã phòng ban không được để trống
        public async Task<Department> GetDepartmentByCodeAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Department code is required");

            var department = await _deptRepository.GetDepartmentByCode(code);
            if (department == null)
                throw new KeyNotFoundException("department is null");

            return department;
        }

        /// <summary>
        /// Lấy danh sách nhân viên theo mã phòng ban
        /// </summary>
        /// <param name="code">Mã phòng ban</param>
        /// <returns>Danh sách nhân viên</returns>
        /// Created by: Anhs (20/04/2026)
        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentCodeAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Department code is required");

            var department = await _deptRepository.GetDepartmentByCode(code.Trim());
            if (department == null)
                throw new KeyNotFoundException("department is null");

            return await _deptRepository.GetEmployeesByDepartmentCode(code.Trim());
        }

        /// <summary>
        /// Đếm số nhân viên trong phòng ban theo mã
        /// </summary>
        /// <param name="code">Mã phòng ban</param>
        /// <returns>Số lượng nhân viên</returns>
        /// Created by: Anhs (20/04/2026)
        public async Task<long> GetEmployeeCountByDepartmentCodeAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Department code is required");

            var department = await _deptRepository.GetDepartmentByCode(code.Trim());
            if (department == null)
                throw new KeyNotFoundException("department is null");

            return await _deptRepository.GetEmployeeCountByDepartmentCode(code.Trim());
        }

        #region OVERRIDE METHODS
        protected override async Task<bool> ValidateBeforeDeleteAsync(Guid entityId)
        {
            // Nếu phòng ban không tồn tại thì cho phép flow delete chạy tiếp để BaseService trả NotFound đúng chuẩn.
            var department = await _baseRepository.GetEntityByIDAsync(entityId);
            if (department == null)
            {
                return true;
            }

            var employeeCount = await _deptRepository.GetEmployeeCountByDepartmentCode(department.DepartmentCode);
            return employeeCount == 0;
        }

        /// <summary>
        /// Validate tùy chỉnh cho Department
        /// </summary>
        protected override List<ValidationError> ValidateCustom(Department department)
        {
            var errors = new List<ValidationError>();

            // Ví dụ: Kiểm tra mã phòng ban không được vượt quá 20 ký tự
            if (!string.IsNullOrEmpty(department.DepartmentCode) && department.DepartmentCode.Length > 20)
            {
                errors.Add(new ValidationError("DepartmentCode", "Mã phòng ban không được vượt quá 20 ký tự"));
            }

            return errors;
        }
        #endregion OVERRIDE METHODS
    }
}
