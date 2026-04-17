using FresherMisa2026.Application.Interfaces;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Application.Interfaces.Services;
using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Employee;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Net.Mail;
using FresherMisa2026.Entities.Enums;

namespace FresherMisa2026.Application.Services
{
    public class EmployeeService : BaseService<Employee>, IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IPositionRepository _positionRepository;

        public EmployeeService(
            IBaseRepository<Employee> baseRepository,
            IEmployeeRepository employeeRepository,
            IDepartmentRepository departmentRepository,
            IPositionRepository positionRepository
            ) : base(baseRepository)
        {
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
            _positionRepository = positionRepository;
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

        public async Task<IEnumerable<Employee>> FilterEmployeesAsync(Guid? departmentId, Guid? positionId, decimal? salaryFrom, decimal? salaryTo, int? gender, DateTime? hireDateFrom, DateTime? hireDateTo)
        {
            return await _employeeRepository.FilterEmployees(departmentId, positionId, salaryFrom, salaryTo, gender, hireDateFrom, hireDateTo);
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

            // 1. EmployeeCode không được trùng
            if (!string.IsNullOrEmpty(employee.EmployeeCode))
            {
                var existing = _employeeRepository.GetEmployeeByCode(employee.EmployeeCode).GetAwaiter().GetResult();
                if (existing != null && (employee.State == ModelSate.Add || existing.EmployeeID != employee.EmployeeID))
                {
                    errors.Add(new ValidationError("EmployeeCode", "Mã nhân viên đã tồn tại"));
                }
            }

            // 2. Email đúng định dạng (nếu có)
            if (!string.IsNullOrEmpty(employee.Email))
            {
                try
                {
                    var _ = new MailAddress(employee.Email);
                }
                catch
                {
                    errors.Add(new ValidationError("Email", "Email không đúng định dạng"));
                }
            }

            // 3. Số điện thoại đúng định dạng (nếu có)
            if (!string.IsNullOrEmpty(employee.PhoneNumber))
            {
                var phoneRegex = new Regex("^\\+?\\d{7,15}$");
                if (!phoneRegex.IsMatch(employee.PhoneNumber))
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

            // 5. DepartmentID và PositionID phải tồn tại
            if (employee.DepartmentID != Guid.Empty)
            {
                var dept = _departmentRepository.GetEntityByIDAsync(employee.DepartmentID).GetAwaiter().GetResult();
                if (dept == null)
                {
                    errors.Add(new ValidationError("DepartmentID", "Phòng ban không tồn tại"));
                }
            }

            if (employee.PositionID != Guid.Empty)
            {
                var pos = _positionRepository.GetEntityByIDAsync(employee.PositionID).GetAwaiter().GetResult();
                if (pos == null)
                {
                    errors.Add(new ValidationError("PositionID", "Vị trí không tồn tại"));
                }
            }

            return errors;
        }
    }
}