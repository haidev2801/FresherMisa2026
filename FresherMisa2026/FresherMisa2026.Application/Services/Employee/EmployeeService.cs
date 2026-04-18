using FresherMisa2026.Application.Interfaces;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Application.Interfaces.Services;
using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Employee;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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

        public async Task<Employee> GetEmployeeByCodeAsync(string code)
        {
            var employee = await _employeeRepository.GetEmployeeByEmployeeCodeAsync(code);
            if (employee == null)
                throw new Exception("Employee not found");

            return employee;
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentIdAsync(Guid departmentId)
        {
            return await _employeeRepository.GetEmployeesByDepartmentId(departmentId);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByPositionIdAsync(Guid positionId)
        {
            return await _employeeRepository.GetEmployeesByPositionId(positionId);
        }

        protected override List<ValidationError> ValidateCustom(Entities.Employee.Employee employee)
        {
            var errors = new List<ValidationError>();
            //Kiểm tra trùng employee code
            var existedEmployeeCode = _employeeRepository.GetEmployeeByEmployeeCodeAsync(employee.EmployeeCode);
            if (existedEmployeeCode.Result != null)
            {
                errors.Add(new ValidationError("EmployeeCode", "Mã nhân viên đã tồn tại"));
            }

            if (!string.IsNullOrEmpty(employee.Email) && !Regex.IsMatch(employee.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase))
            {
                errors.Add(new ValidationError("EmployeeEmail", "Email không đúng định dạng"));
            }
            if (!string.IsNullOrEmpty(employee.PhoneNumber) && !Regex.IsMatch(employee.PhoneNumber, @"^(?:\+84|84|0)(3|5|7|8|9)([0-9]{8})$", RegexOptions.IgnoreCase))
            {
                errors.Add(new ValidationError("EmployeePhoneNumber", "Số điện thoại không hợp lệ"));
            }
            if (!string.IsNullOrEmpty(employee.EmployeeCode) && employee.EmployeeCode.Length > 20)
            {
                errors.Add(new ValidationError("EmployeeCode", "Mã nhân viên không được vượt quá 20 ký tự"));
            }

            if (string.IsNullOrEmpty(employee.EmployeeName))
            {
                errors.Add(new ValidationError("EmployeeName", "Tên nhân viên không được để trống"));
            }

            if (employee.DateOfBirth.HasValue && employee.DateOfBirth.Value.Date >= DateTime.Now.Date)
            {
                errors.Add(new ValidationError("EmployeeName", "Ngày sinh phải nhỏ hơn ngày hiện tại"));
            }

            return errors;
        }
    }
}