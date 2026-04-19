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

            // EmployeeCode max length
            if (!string.IsNullOrWhiteSpace(employee.EmployeeCode) && employee.EmployeeCode.Length > 20)
            {
                errors.Add(new ValidationError("EmployeeCode", "Mã nhân viên không được vượt quá 20 ký tự"));
            }

            // Email format
            if (!string.IsNullOrWhiteSpace(employee.Email))
            {
                var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                if (!System.Text.RegularExpressions.Regex.IsMatch(employee.Email, emailPattern))
                {
                    errors.Add(new ValidationError("Email", "Email không đúng định dạng"));
                }
            }

            // Phone number format (simple VN phone regex)
            if (!string.IsNullOrWhiteSpace(employee.PhoneNumber))
            {
                var phonePattern = @"^(0|\+84)[0-9]{9,10}$";
                if (!System.Text.RegularExpressions.Regex.IsMatch(employee.PhoneNumber, phonePattern))
                {
                    errors.Add(new ValidationError("PhoneNumber", "Số điện thoại không đúng định dạng"));
                }
            }

            // DateOfBirth < Now
            if (employee.DateOfBirth.HasValue && employee.DateOfBirth.Value >= DateTime.Now.Date)
            {
                errors.Add(new ValidationError("DateOfBirth", "Ngày sinh phải nhỏ hơn ngày hiện tại"));
            }

            // Check duplicate EmployeeCode (async, nhưng ở đây chỉ check sync, production nên check DB async)
            var exist = _employeeRepository.GetEmployeeByCode(employee.EmployeeCode).Result;
            if (exist != null && exist.EmployeeID != employee.EmployeeID)
            {
                errors.Add(new ValidationError("EmployeeCode", "Mã nhân viên đã tồn tại"));
            }

            return errors;
        }

        public async Task<PagingResponse<Employee>> FilterEmployeesAsync(
            Guid? departmentId,
            Guid? positionId,
            decimal? salaryFrom,
            decimal? salaryTo,
            int? gender,
            DateTime? hireDateFrom,
            DateTime? hireDateTo)
        {
            var (total, data) = await _employeeRepository.FilterEmployees(
                departmentId, positionId, salaryFrom, salaryTo, gender, hireDateFrom, hireDateTo);
            return new PagingResponse<Employee>
            {
                Total = total,
                Data = data.ToList()
            };
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentCodeAsync(string departmentCode)
        {
            return await _employeeRepository.GetEmployeesByDepartmentCode(departmentCode);
        }

        public async Task<int> CountEmployeesByDepartmentCodeAsync(string departmentCode)
        {
            return await _employeeRepository.CountEmployeesByDepartmentCode(departmentCode);
        }
    }
}