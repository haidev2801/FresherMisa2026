using FresherMisa2026.Application.Interfaces;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Application.Interfaces.Services;
using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Employee;
using System;
using System.Collections.Generic;

namespace FresherMisa2026.Application.Services
{
    public class EmployeeService : BaseService<Employee>, IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(
            IBaseRepository<Employee> baseRepository,
            IEmployeeRepository employeeRepository
            ) : base(baseRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<Employee?> GetEmployeeByCodeAsync(string code)
        {
            return await _employeeRepository.GetEmployeeByCode(code);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentIdAsync(Guid departmentId)
        {
            return await _employeeRepository.GetEmployeesByDepartmentId(departmentId);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByPositionIdAsync(Guid positionId)
        {
            return await _employeeRepository.GetEmployeesByPositionId(positionId);
        }
        /// <summary>
        /// Hàm lấy danh sách nhân viên theo các tiêu chí lọc
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="positionId"></param>
        /// <param name="salaryFrom"></param>
        /// <param name="salaryTo"></param>
        /// <param name="gender"></param>
        /// <param name="hireDateFrom"></param>
        /// <param name="hireDateTo"></param>
        /// <returns></returns>
        public async Task<PagingResponse<Employee>> GetEmployeesFilterAsync(
            Guid? departmentId,
            Guid? positionId,
            string? salaryFrom,
            string? salaryTo,
            int? gender,
            DateTime? hireDateFrom,
            DateTime? hireDateTo,
            int pageSize,
            int pageIndex)
        {
            return await _employeeRepository.GetEmployeesFilterAsync(
                departmentId,
                positionId,
                salaryFrom,
                salaryTo,
                gender,
                hireDateFrom,
                hireDateTo,
                pageSize,
                pageIndex);
        }

        protected override List<ValidationError> ValidateCustom(Employee employee)
        {
            var errors = new List<ValidationError>();

            if (!string.IsNullOrEmpty(employee.EmployeeCode) && employee.EmployeeCode.Length > 20)
            {
                errors.Add(new ValidationError("EmployeeCode", "Mã nhân viên không được vượt quá 20 ký tự"));
            }

            if (!string.IsNullOrEmpty(employee.EmployeeCode))
            {
                var existingEmployee = _employeeRepository.GetEmployeeByCode(employee.EmployeeCode).Result;
                if (existingEmployee != null && existingEmployee.EmployeeID != employee.EmployeeID)
                {
                    errors.Add(new ValidationError("EmployeeCode", "Mã nhân viên đã tồn tại"));
                }
            }

            if (!string.IsNullOrEmpty(employee.Email)) { 
                var regex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                if (!System.Text.RegularExpressions.Regex.IsMatch(employee.Email, regex))
                {
                    errors.Add(new ValidationError("Email", "Email không hợp lệ"));
                }
            }

            if (!string.IsNullOrEmpty(employee.PhoneNumber))
            {
                var regex = @"^\d{10}$";
                if (!System.Text.RegularExpressions.Regex.IsMatch(employee.PhoneNumber, regex))
                {
                    errors.Add(new ValidationError("PhoneNumber", "Số điện thoại không hợp lệ"));
                }
            }

            if (employee.DateOfBirth.HasValue)
            {
                if (employee.DateOfBirth.Value.Date > DateTime.Now.Date)
                {
                    errors.Add(new ValidationError("DateOfBirth", "Ngày sinh không hợp lệ"));
                }
            }

            return errors;
        }
    }
}
