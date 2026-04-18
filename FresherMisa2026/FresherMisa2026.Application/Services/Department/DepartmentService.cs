using FresherMisa2026.Application.Interfaces;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Application.Interfaces.Services;
using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Department;
using FresherMisa2026.Entities.Enums;
using System;
using System.Collections.Generic;
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
        /// <returns></returns>
        /// Created By: dvhai (10/04/2026)
        public async Task<ServiceResponse> GetDepartmentByCodeAsync(string code)
        {
            // kiểm tra xem code có null hay rỗng không 
            if (string.IsNullOrEmpty(code)){
                return CreateErrorResponse(ResponseCode.BadRequest, "DepartmentCode không được trống", "Mã phòng ban không được để trống");
            }
            var department = await _deptRepository.GetDepartmentByCodeAsync(code);
            if (department == null)
                return CreateErrorResponse(ResponseCode.NotFound, "DepartmentCode không tồn tại", "Không tìm thấy phòng ban");

            return CreateSuccessResponse(ResponseCode.Success, department);
        }

        public async Task<ServiceResponse> GetEmployeeCountByDepartmentCode(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return CreateErrorResponse(ResponseCode.BadRequest, "DepartmentCode không được trống", "Mã phòng ban không được để trống");
            }
            var employees = await _employeeRepository.GetEmployeesByDepartmentCodeAsync(code);
            return CreateSuccessResponse(ResponseCode.Success, employees.Count());
        }

        public async Task<ServiceResponse> GetEmployeesByDepartmentCode(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return CreateErrorResponse(ResponseCode.BadRequest, "DepartmentCode không được trống", "Mã phòng ban không được để trống");
            }
            var employees = await _employeeRepository.GetEmployeesByDepartmentCodeAsync(code);
            if (employees.Count() <= 0)
            {
                return CreateErrorResponse(ResponseCode.NotFound, "", "Không tìm thấy nhân viên nào");
            }
            return CreateSuccessResponse(ResponseCode.Success, employees);
        }

        #region OVERRIDE METHODS
        protected override async Task<bool> ValidateBeforeDeleteAsync(Guid entityId)
        {
            //1. Validate còn nhân viên trong phòng ban không
            var hasEmployee = await _employeeRepository.GetEmployeesByDepartmentId(entityId);
            
            return !(hasEmployee.Count() > 0);
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
