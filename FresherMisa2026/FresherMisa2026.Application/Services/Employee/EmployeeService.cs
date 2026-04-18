using FresherMisa2026.Application.Interfaces;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Application.Interfaces.Services;
using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Employee;
using FresherMisa2026.Entities.Employee.DTO;
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

        public async Task<IEnumerable<Employee>> FilterEmployeesAsync(EmployeeFilterRequest filterRequest)
        {
            return await _employeeRepository.FilterEmployeesAsync(filterRequest);
        }

        protected override List<ValidationError> ValidateCustom(Employee employee)
        {
            var errors = new List<ValidationError>();

            if (!string.IsNullOrEmpty(employee.EmployeeCode) && employee.EmployeeCode.Length > 20)
            {
                errors.Add(new ValidationError("EmployeeCode", "Mã nhân viên không được vượt quá 20 ký tự"));
            }

            if (string.IsNullOrEmpty(employee.EmployeeName))
            {
                errors.Add(new ValidationError("EmployeeName", "Tên nhân viên không được để trống"));
            }

            if (employee.DepartmentID == Guid.Empty)
            {
                errors.Add(new ValidationError("DepartmentID", "Trường Phòng ban bắt buộc nhập"));
            }

            if (employee.PositionID == Guid.Empty)
            {
                errors.Add(new ValidationError("PositionID", "Trường Vị trí bắt buộc nhập"));
            }

            if (!string.IsNullOrWhiteSpace(employee.EmployeeCode))
            {
                var existedEmployee = _employeeRepository
                    .GetEmployeeByCode(employee.EmployeeCode)
                    .GetAwaiter()
                    .GetResult();

                if (existedEmployee != null && existedEmployee.EmployeeID != employee.EmployeeID)
                {
                    errors.Add(new ValidationError("EmployeeCode", "Mã nhân viên đã tồn tại"));
                }
            }

            if (!string.IsNullOrWhiteSpace(employee.Email) && !IsValidEmail(employee.Email))
            {
                errors.Add(new ValidationError("Email", "Email không đúng định dạng"));
            }

            if (!string.IsNullOrWhiteSpace(employee.PhoneNumber) && !IsValidPhoneNumber(employee.PhoneNumber))
            {
                errors.Add(new ValidationError("PhoneNumber", "Số điện thoại không đúng định dạng"));
            }

            if (employee.DateOfBirth.HasValue && employee.DateOfBirth.Value.Date >= DateTime.Now.Date)
            {
                errors.Add(new ValidationError("DateOfBirth", "Ngày sinh phải nhỏ hơn ngày hiện tại"));
            }

            return errors;
        }

        private static bool IsValidEmail(string email)
        {
            const string emailPattern = @"^[^\s@]+@[^\s@]+\.[^\s@]+$";
            return Regex.IsMatch(email, emailPattern, RegexOptions.IgnoreCase);
        }

        private static bool IsValidPhoneNumber(string phoneNumber)
        {
            const string phonePattern = @"^\+?\d{9,15}$";
            return Regex.IsMatch(phoneNumber, phonePattern);
        }
    }
}