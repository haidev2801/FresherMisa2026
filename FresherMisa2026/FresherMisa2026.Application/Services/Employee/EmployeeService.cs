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

            if (!string.IsNullOrEmpty(employee.EmployeeCode) && _employeeRepository.GetEmployeeByCode(employee.EmployeeCode) != null)
            {
                errors.Add(new ValidationError("EmployeeCode", "Mã nhân viên đã tồn tại"));
            }

            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!string.IsNullOrEmpty(employee.Email) && !Regex.IsMatch(employee.Email, emailPattern))
            {
                errors.Add(new ValidationError("Email", "Email không hợp lệ"));
            }

            string phonePattern = @"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$";
            if (!string.IsNullOrEmpty(employee.PhoneNumber) && !Regex.IsMatch(employee.PhoneNumber, phonePattern))
            {
                errors.Add(new ValidationError("Phone", "Số điện thoại không hợp lệ"));
            }

            if (employee.DateOfBirth != null && employee.DateOfBirth > DateTime.Now)
            {
                errors.Add(new ValidationError("DateOfBirth", "Ngày sinh không được lớn hơn ngày hiện tại"));
            }

            return errors;
        }
    }
}