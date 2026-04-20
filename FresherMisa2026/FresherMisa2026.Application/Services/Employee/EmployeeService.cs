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
        private const int MaxEmployeeCodeLength = 20;
        private static readonly Regex EmailRegex = new(@"^[^\s@]+@[^\s@]+\.[^\s@]+$", RegexOptions.Compiled);
        private static readonly Regex PhoneNumberRegex = new(@"^\+?[0-9]{8,15}$", RegexOptions.Compiled);

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
            {
                throw new Exception("Employee not found");
            }

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

        public async Task<IEnumerable<Employee>> FilterEmployeesAsync(EmployeeFilterRequest request)
        {
            return await _employeeRepository.FilterEmployees(request);
        }

        protected override async Task<List<ValidationError>> ValidateCustomAsync(Employee employee)
        {
            var errors = new List<ValidationError>();

            employee.EmployeeCode = employee.EmployeeCode?.Trim();
            employee.EmployeeName = employee.EmployeeName?.Trim();
            employee.Email = employee.Email?.Trim();
            employee.PhoneNumber = employee.PhoneNumber?.Trim();

            if (string.IsNullOrWhiteSpace(employee.EmployeeCode))
            {
                errors.Add(new ValidationError("EmployeeCode", "Mã nhân viên không được để trống"));
            }

            if (!string.IsNullOrEmpty(employee.EmployeeCode) && employee.EmployeeCode.Length > MaxEmployeeCodeLength)
            {
                errors.Add(new ValidationError("EmployeeCode", "Mã nhân viên không được vượt quá 20 ký tự"));
            }

            if (!string.IsNullOrEmpty(employee.EmployeeCode))
            {
                var existedEmployee = await _employeeRepository.GetEmployeeByCode(employee.EmployeeCode);
                var isDuplicate = existedEmployee != null && employee.State switch
                {
                    ModelSate.Add => true,
                    ModelSate.Update => existedEmployee.EmployeeID != employee.EmployeeID,
                    _ => false
                };

                if (isDuplicate)
                {
                    errors.Add(new ValidationError("EmployeeCode", "Mã nhân viên đã tồn tại"));
                }
            }

            if (string.IsNullOrWhiteSpace(employee.EmployeeName))
            {
                errors.Add(new ValidationError("EmployeeName", "Tên nhân viên không được để trống"));
            }

            if (!string.IsNullOrWhiteSpace(employee.Email) && !EmailRegex.IsMatch(employee.Email))
            {
                errors.Add(new ValidationError("Email", "Email không đúng định dạng"));
            }

            if (!string.IsNullOrWhiteSpace(employee.PhoneNumber) && !PhoneNumberRegex.IsMatch(employee.PhoneNumber))
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
