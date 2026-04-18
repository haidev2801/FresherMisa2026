using FresherMisa2026.Application.Interfaces;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Application.Interfaces.Services;
using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Department;
using FresherMisa2026.Entities.Employee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FresherMisa2026.Application.Services
{
    public class DepartmentService : BaseService<Department>, IDepartmentSerice
    {
        private readonly IDepartmentRepository _deptRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public DepartmentService(
            IBaseRepository<Department> baseRepository,
            IDepartmentRepository departmentRepository,
            IEmployeeRepository employeeRepository
            ) : base(baseRepository)
        {
            _deptRepository = departmentRepository;
            _employeeRepository = employeeRepository;
        }

        /// <summary>
        /// Lấy department theo code
        /// </summary>
        /// Created By: dvhai (10/04/2026)
        public async Task<Department> GetDepartmentByCodeAsync(string code)
        {
            var department = await _deptRepository.GetDepartmentByCode(code);
            if (department == null)
                throw new Exception("department is null");

            return department;
        }

        /// <summary>
        /// Lấy danh sách nhân viên theo mã phòng ban
        /// </summary>
        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentCodeAsync(string code)
        {
            // 1. Tìm department theo code
            var department = await _deptRepository.GetDepartmentByCode(code);
            if (department == null)
                throw new Exception($"Không tìm thấy phòng ban có mã: {code}");

            // 2. Lấy danh sách nhân viên theo DepartmentID
            return await _employeeRepository.GetEmployeesByDepartmentId(department.DepartmentID);
        }

        /// <summary>
        /// Đếm số nhân viên trong phòng ban theo mã phòng ban
        /// </summary>
        public async Task<int> GetEmployeeCountByDepartmentCodeAsync(string code)
        {
            var employees = await GetEmployeesByDepartmentCodeAsync(code);
            return employees.Count();
        }

        #region OVERRIDE METHODS
        protected override async Task<bool> ValidateBeforeDeleteAsync(Guid entityId)
        {
            //1. Validate còn nhân viên trong phòng ban không
            bool hasEmployee = true;

            return !hasEmployee;
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
