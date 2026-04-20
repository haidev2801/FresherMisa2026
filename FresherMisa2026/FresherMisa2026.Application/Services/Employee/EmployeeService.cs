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

        public async Task<IEnumerable<Employee>> FilterEmployeesAsync(
            Guid? departmentId,
            Guid? positionId,
            decimal? salaryFrom,
            decimal? salaryTo,
            int? gender,
            DateTime? hireDateFrom,
            DateTime? hireDateTo)
        {
            return await _employeeRepository.FilterEmployeesAsync(
                departmentId, positionId, salaryFrom, salaryTo, gender, hireDateFrom, hireDateTo);
        }

        /// <summary>
        /// Task 3.3: Lọc nhân viên có phân trang
        /// </summary>
        public async Task<PagingResponse<Employee>> FilterEmployeesWithPagingAsync(
            Guid? departmentId,
            Guid? positionId,
            decimal? salaryFrom,
            decimal? salaryTo,
            int? gender,
            DateTime? hireDateFrom,
            DateTime? hireDateTo,
            int pageSize,
            int pageIndex)
        {
            var (total, data) = await _employeeRepository.FilterEmployeesWithPagingAsync(
                departmentId, positionId, salaryFrom, salaryTo, gender, hireDateFrom, hireDateTo,
                pageSize, pageIndex);

            return new PagingResponse<Employee>
            {
                Total = total,
                Data = data.ToList()
            };
        }

        /// <summary>
        /// Validate tùy chỉnh cho Employee
        /// - Mã nhân viên không được vượt quá 20 ký tự
        /// - Mã nhân viên không được trùng lặp
        /// - Email phải đúng định dạng (nếu có)
        /// - Số điện thoại phải đúng định dạng (nếu có)
        /// - Ngày sinh phải nhỏ hơn ngày hiện tại
        /// </summary>
        protected override List<ValidationError> ValidateCustom(Employee employee)
        {
            var errors = new List<ValidationError>();

            // 1. Mã nhân viên không được vượt quá 20 ký tự
            if (!string.IsNullOrEmpty(employee.EmployeeCode) && employee.EmployeeCode.Length > 20)
            {
                errors.Add(new ValidationError("EmployeeCode", "Mã nhân viên không được vượt quá 20 ký tự"));
            }

            // 2. Email phải đúng định dạng (nếu có)
            if (!string.IsNullOrEmpty(employee.Email))
            {
                var emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                if (!Regex.IsMatch(employee.Email, emailPattern))
                {
                    errors.Add(new ValidationError("Email", "Email không đúng định dạng"));
                }
            }

            // 3. Số điện thoại phải đúng định dạng (nếu có)
            if (!string.IsNullOrEmpty(employee.PhoneNumber))
            {
                var phonePattern = @"^(0|\+84)[0-9]{9,10}$";
                if (!Regex.IsMatch(employee.PhoneNumber, phonePattern))
                {
                    errors.Add(new ValidationError("PhoneNumber", "Số điện thoại không đúng định dạng"));
                }
            }

            // 4. Ngày sinh phải nhỏ hơn ngày hiện tại
            if (employee.DateOfBirth.HasValue && employee.DateOfBirth.Value >= DateTime.Now)
            {
                errors.Add(new ValidationError("DateOfBirth", "Ngày sinh phải nhỏ hơn ngày hiện tại"));
            }

            // 5. Mã nhân viên không được trùng lặp (kiểm tra async trong ValidateCustomAsync sẽ tốt hơn,
            //    nhưng do pattern hiện tại dùng sync, ta check tại đây bằng cách gọi .Result)
            if (!string.IsNullOrEmpty(employee.EmployeeCode))
            {
                var existingEmployee = _employeeRepository.GetEmployeeByCode(employee.EmployeeCode).Result;
                if (existingEmployee != null)
                {
                    // Nếu đang thêm mới (State = Add) → luôn báo lỗi trùng
                    // Nếu đang cập nhật (State = Update) → chỉ báo lỗi nếu ID khác
                    if (employee.State == ModelSate.Add ||
                        (employee.State == ModelSate.Update && existingEmployee.EmployeeID != employee.EmployeeID))
                    {
                        errors.Add(new ValidationError("EmployeeCode", "Mã nhân viên đã tồn tại"));
                    }
                }
            }

            return errors;
        }
    }
}