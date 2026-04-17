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
            var employee = await _employeeRepository.GetEmployeeByCode(code);
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

        protected override List<ValidationError> ValidateCustom(Employee employee)
        {
            var errors = new List<ValidationError>();

            if (!string.IsNullOrEmpty(employee.EmployeeCode) && employee.EmployeeCode.Length > 20)
            {
                errors.Add(new ValidationError("EmployeeCode", "M\u00E3 nh\u00E2n vi\u00EAn kh\u00F4ng \u0111\u01B0\u1EE3c v\u01B0\u1EE3t qu\u00E1 20 k\u00FD t\u1EF1"));
            }

            if (string.IsNullOrEmpty(employee.EmployeeName))
            {
                errors.Add(new ValidationError("EmployeeName", "T\u00EAn nh\u00E2n vi\u00EAn kh\u00F4ng \u0111\u01B0\u1EE3c \u0111\u1EC3 tr\u1ED1ng"));
            }

            if (employee.DepartmentID == Guid.Empty)
            {
                errors.Add(new ValidationError("DepartmentID", "Ph\u00F2ng ban b\u1EAFt bu\u1ED9c ch\u1ECDn"));
            }

            if (employee.PositionID == Guid.Empty)
            {
                errors.Add(new ValidationError("PositionID", "V\u1ECB tr\u00ED b\u1EAFt bu\u1ED9c ch\u1ECDn"));
            }

            if (!string.IsNullOrEmpty(employee.Email) &&
                !Regex.IsMatch(employee.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                errors.Add(new ValidationError("Email", "Email không đúng định dạng"));
            }

            if (!string.IsNullOrEmpty(employee.PhoneNumber) &&
                !Regex.IsMatch(employee.PhoneNumber, @"^(0[3|5|7|8|9])+([0-9]{8})$"))
            {
                errors.Add(new ValidationError("PhoneNumber", "Số điện thoại không đúng định dạng"));
            }

            if (employee.DateOfBirth.HasValue && employee.DateOfBirth.Value.Date >= DateTime.Today)
            {
                errors.Add(new ValidationError("DateOfBirth", "Ngày sinh phải nhỏ hơn ngày hiện tại"));
            }

            return errors;
        }
    }
}
