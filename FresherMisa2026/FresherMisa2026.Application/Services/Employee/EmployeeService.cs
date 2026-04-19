using FresherMisa2026.Application.Interfaces;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Application.Interfaces.Services;
using FresherMisa2026.Application.DTOs.Employee;
using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Enums;
using FresherMisa2026.Entities.Employee;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FresherMisa2026.Application.Services
{
    public class EmployeeService : BaseService<Employee>, IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        /// <summary>
        /// Regex pattern cho email hợp lệ
        /// </summary>
        private static readonly Regex EmailRegex = new Regex(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// Regex pattern cho số điện thoại VN (10-11 số, bắt đầu bằng 0 hoặc +84)
        /// </summary>
        private static readonly Regex PhoneRegex = new Regex(
            @"^(\+84|0)\d{9,10}$",
            RegexOptions.Compiled);

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

        public async Task<ServiceResponse> FilterEmployeesAsync(EmployeeFilterRequest filterRequest)
        {
            filterRequest ??= new EmployeeFilterRequest();

            if (filterRequest.Gender.HasValue && (filterRequest.Gender < 0 || filterRequest.Gender > 2))
            {
                return CreateErrorResponse(ResponseCode.BadRequest, "Giới tính không hợp lệ. Chi chap nhan 0, 1, 2");
            }

            if (filterRequest.SalaryFrom.HasValue && filterRequest.SalaryTo.HasValue
                && filterRequest.SalaryFrom > filterRequest.SalaryTo)
            {
                return CreateErrorResponse(ResponseCode.BadRequest, "Mức lương bắt đầu phải nhỏ hơn mức lương kết thúc");
            }

            if (filterRequest.HireDateFrom.HasValue && filterRequest.HireDateTo.HasValue
                && filterRequest.HireDateFrom > filterRequest.HireDateTo)
            {
                return CreateErrorResponse(ResponseCode.BadRequest, "Ngày bắt đầu phải nhỏ hơn ngày kết thúc");
            }

            if (filterRequest.PageSize <= 0)
            {
                filterRequest.PageSize = 20;
            }

            if (filterRequest.PageIndex <= 0)
            {
                filterRequest.PageIndex = 1;
            }

            var (total, data) = await _employeeRepository.FilterEmployeesAsync(filterRequest);

            var response = new PagingResponse<Employee>
            {
                Total = total,
                PageSize = filterRequest.PageSize,
                PageIndex = filterRequest.PageIndex,
                Data = data.ToList()
            };

            return CreateSuccessResponse(response);
        }

        /// <summary>
        /// Validate tùy chỉnh đồng bộ cho Employee (format, độ dài, ngày tháng)
        /// </summary>
        /// <param name="employee">Thực thể Employee</param>
        /// <returns>Danh sách lỗi validate</returns>
        /// UPDATED: Anhs(19/04/2026): Thêm validate ngày sinh phải nhỏ hơn ngày hiện tại
        protected override List<ValidationError> ValidateCustom(Employee employee)
        {
            var errors = new List<ValidationError>();

            // 1. Mã nhân viên không được vượt quá 20 ký tự
            if (!string.IsNullOrEmpty(employee.EmployeeCode) && employee.EmployeeCode.Length > 20)
            {
                errors.Add(new ValidationError("EmployeeCode", "Mã nhân viên không được vượt quá 20 ký tự"));
            }

            // 2. Email phải đúng định dạng (nếu có)
            if (!string.IsNullOrEmpty(employee.Email) && !EmailRegex.IsMatch(employee.Email))
            {
                errors.Add(new ValidationError("Email", "Email không đúng định dạng"));
            }

            // 3. Số điện thoại phải đúng định dạng (nếu có)
            if (!string.IsNullOrEmpty(employee.PhoneNumber) && !PhoneRegex.IsMatch(employee.PhoneNumber))
            {
                errors.Add(new ValidationError("PhoneNumber", "Số điện thoại không đúng định dạng"));
            }

            // 4. Ngày sinh phải nhỏ hơn ngày hiện tại
            if (employee.DateOfBirth.HasValue && employee.DateOfBirth.Value >= DateTime.Now)
            {
                errors.Add(new ValidationError("DateOfBirth", "Ngày sinh phải nhỏ hơn ngày hiện tại"));
            }

            return errors;
        }

        /// <summary>
        /// Validate bất đồng bộ - kiểm tra mã nhân viên trùng lặp 
        /// </summary>
        /// <param name="employee">Thực thể Employee</param>
        /// <returns>Danh sách lỗi validate</returns>
        /// CREATED BY: Anhs(19/04/2026): Thêm validate mã nhân viên trùng lặp
        protected override async Task<List<ValidationError>> ValidateCustomAsync(Employee employee)
        {
            var errors = new List<ValidationError>();

            // Kiểm tra mã nhân viên trùng lặp
            if (!string.IsNullOrEmpty(employee.EmployeeCode))
            {
                var existingEmployee = await _employeeRepository.GetEmployeeByCode(employee.EmployeeCode);
                if (existingEmployee != null)
                {
                    // Nếu đang thêm mới (Add) → luôn báo trùng
                    // Nếu đang cập nhật (Update) → chỉ báo trùng khi ID khác nhau
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