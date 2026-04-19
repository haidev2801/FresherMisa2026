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

            // 1. EmployeeCode không được quá 20 ký tự
            if (!string.IsNullOrEmpty(employee.EmployeeCode) && employee.EmployeeCode.Length > 20)
            {
                errors.Add(new ValidationError("EmployeeCode", "Mã nhân viên không được vượt quá 20 ký tự"));
            }

            // 2. EmployeeName bắt buộc
            if (string.IsNullOrEmpty(employee.EmployeeName))
            {
                errors.Add(new ValidationError("EmployeeName", "Tên nhân viên không được để trống"));
            }

            // 3. Validate Email
            if (!string.IsNullOrEmpty(employee.Email))
            {
                try
                {
                    var addr = new System.Net.Mail.MailAddress(employee.Email);
                }
                catch
                {
                    errors.Add(new ValidationError("Email", "Email không đúng định dạng"));
                }
            }

            // 4. Validate Số điện thoại
            if (!string.IsNullOrEmpty(employee.PhoneNumber))
            {
                var phoneRegex = new System.Text.RegularExpressions.Regex(@"^(0|\+84)[0-9]{9}$");

                if (!phoneRegex.IsMatch(employee.PhoneNumber))
                {
                    errors.Add(new ValidationError("PhoneNumber", "Số điện thoại không đúng định dạng"));
                }
            }

            // 5. Validate ngày sinh < hiện tại
            if (employee.DateOfBirth.HasValue)
            {
                if (employee.DateOfBirth.Value >= DateTime.Now)
                {
                    errors.Add(new ValidationError("DateOfBirth", "Ngày sinh phải nhỏ hơn ngày hiện tại"));
                }
            }

            return errors;
        }

        public async Task<IEnumerable<Employee>> FilterEmployeesAsync(EmployeeFilterRequest request)
        {
            return await _employeeRepository.FilterEmployees(request);
        }
    }
}