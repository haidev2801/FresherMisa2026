using FresherMisa2026.Application.Interfaces;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Application.Interfaces.Services;
using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Employee;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<PagingResponse<Employee>> GetEmployeesByFilterAsync(Guid? departmentId, Guid? positionId, decimal? salaryFrom, decimal? salaryTo, int? gender, DateTime? hireDateFrom, DateTime? hireDateTo, int pageSize = 10, int pageIndex = 1)
        {
            // Get full filtered list from repository
            var data = (await _employeeRepository.GetEmployeesByFilter(departmentId, positionId, salaryFrom, salaryTo, gender, hireDateFrom, hireDateTo)).ToList();

            var total = data.Count;
            var paged = data.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            var response = new PagingResponse<Employee>
            {
                Total = total,
                PageSize = pageSize,
                PageIndex = pageIndex,
                Data = paged
            };

            return response;
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
                errors.Add(new ValidationError("EmployeeCode", "Mã nhân viên không được vượt quá 20 ký tự"));
            }

            if (string.IsNullOrEmpty(employee.EmployeeName))
            {
                errors.Add(new ValidationError("EmployeeName", "Tên nhân viên không được để trống"));
            }

            // 1. Kiểm tra mã nhân viên không được trùng lặp
            if (!string.IsNullOrEmpty(employee.EmployeeCode))
            {
                try
                {
                    var existing = _employeeRepository.GetEmployeeByCode(employee.EmployeeCode).GetAwaiter().GetResult();
                    if (existing != null && existing.EmployeeID != employee.EmployeeID)
                    {
                        errors.Add(new ValidationError("EmployeeCode", "Mã nhân viên đã tồn tại"));
                    }
                }
                catch
                {
                    // ignore repo errors here; they will be surfaced elsewhere
                }
            }

            // 2. Email đúng định dạng (nếu có)
            if (!string.IsNullOrEmpty(employee.Email))
            {
                var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                if (!Regex.IsMatch(employee.Email, emailPattern, RegexOptions.IgnoreCase))
                {
                    errors.Add(new ValidationError("Email", "Email không đúng định dạng"));
                }
            }

            // 3. Số điện thoại đúng định dạng (nếu có) - chỉ chứa chữ số, độ dài 9-11
            if (!string.IsNullOrEmpty(employee.PhoneNumber))
            {
                var phonePattern = "^[0-9]{9,11}$";
                if (!Regex.IsMatch(employee.PhoneNumber, phonePattern))
                {
                    errors.Add(new ValidationError("PhoneNumber", "Số điện thoại không đúng định dạng"));
                }
            }

            // 4. Ngày sinh phải nhỏ hơn ngày hiện tại
            if (employee.DateOfBirth.HasValue)
            {
                if (employee.DateOfBirth.Value.Date >= DateTime.Now.Date)
                {
                    errors.Add(new ValidationError("DateOfBirth", "Ngày sinh phải nhỏ hơn ngày hiện tại"));
                }
            }

            return errors;
        }
    }
}