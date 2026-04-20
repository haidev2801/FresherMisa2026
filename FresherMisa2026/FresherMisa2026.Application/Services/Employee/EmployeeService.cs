using FresherMisa2026.Application.Exceptions;
using FresherMisa2026.Application.Interfaces;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Application.Interfaces.Services;
using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Employee;
using FresherMisa2026.Entities.Enums;
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
                throw new KeyNotFoundException("Employee not found");

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


        /// <summary>
        /// Validate tùy chỉnh cho Employee
        /// </summary>
        /// <param name="employee">Employee</param>
        /// <returns>Danh sách lỗi</returns>
        /// Created By: NgoHai (16/04/2026)
        protected override List<ValidationError> ValidateCustom(Employee employee)
        {
            var errors = new List<ValidationError>();

            // Validate mã nhân viên
            if (!string.IsNullOrEmpty(employee.EmployeeCode) && employee.EmployeeCode.Length > 20)
            {
                errors.Add(new ValidationError("EmployeeCode", "Mã nhân viên không được vượt quá 20 ký tự"));
            }

            // Validate email
            if (!string.IsNullOrEmpty(employee.Email))
            {
                    const string emailPattern = @"^[^\s@]+@[^\s@]+\.[^\s@]+$";
                    if (!Regex.IsMatch(employee.Email, emailPattern))
                    {
                        errors.Add(new ValidationError("Email", "Email không đúng định dạng"));
                    }
            }

            // Validate số điện thoại
            if (!string.IsNullOrEmpty(employee.PhoneNumber))
            {
                const string phonePattern = @"^(0|\+84)[0-9]{9,10}$";
                if (!Regex.IsMatch(employee.PhoneNumber, phonePattern))
                {
                    errors.Add(new ValidationError("PhoneNumber", "Số điện thoại không đúng định dạng"));
                }

                if (employee.PhoneNumber.Length > 11 || employee.PhoneNumber.Length < 10)
                {
                    errors.Add(new ValidationError("PhoneNumber", "Số điện thoại không được vượt quá 11 ký tự và không được ít hơn 10 ký tự"));
                }
            }

            // Validate ngày sinh
            if (employee.DateOfBirth.HasValue && employee.DateOfBirth.Value >= DateTime.Now)
            {
                errors.Add(new ValidationError("DateOfBirth", "Ngày sinh phải nhỏ hơn ngày hiện tại"));
            }

            if (employee.HireDate.HasValue && employee.DateOfBirth.HasValue
                && employee.HireDate.Value.Date < employee.DateOfBirth.Value.Date)
            {
                errors.Add(new ValidationError("HireDate", "Ngày vào làm không được trước ngày sinh"));
            }

            // Validate mã nhân viên không trùng
            if (!string.IsNullOrEmpty(employee.EmployeeCode))
            {
                var employeeByCode = _employeeRepository.GetEmployeeByCode(employee.EmployeeCode).GetAwaiter().GetResult();
                var isDuplicateCode = employeeByCode != null && (employee.State == ModelSate.Add || employeeByCode.EmployeeID != employee.EmployeeID);
                if (isDuplicateCode)
                {
                    errors.Add(new ValidationError("EmployeeCode", "Mã nhân viên đã tồn tại"));
                }
            }

            return errors;
        }

        public async Task<ServiceResponse> GetEmployeesByFilterAsync(EmployeeFilterRequest filter)
        {
            if (filter.SalaryFrom.HasValue && filter.SalaryTo.HasValue && filter.SalaryFrom > filter.SalaryTo)
            {
                return CreateErrorResponse(ResponseCode.BadRequest, "Lương từ phải nhỏ hơn hoặc bằng lương đến");
            }

            if (filter.HireDateFrom.HasValue && filter.HireDateTo.HasValue && filter.HireDateFrom > filter.HireDateTo)
            {
                return CreateErrorResponse(ResponseCode.BadRequest, "Ngày vào làm từ phải nhỏ hơn hoặc bằng ngày vào làm đến");
            }

            // Query string không gửi pageIndex/pageSize thì binder gán 0 — coi như trang 1, size 10.
            var pageIndex = filter.PageIndex < 1 ? 1 : filter.PageIndex;
            var pageSize = filter.PageSize < 1 ? 10 : filter.PageSize;
            if (pageSize > 200)
            {
                return CreateErrorResponse(ResponseCode.BadRequest, "pageSize không hợp lệ", "pageSize tối đa 200");
            }

            filter.PageIndex = pageIndex;
            filter.PageSize = pageSize;

            var (total, data) = await _employeeRepository.GetEmployeesByFilterAsync(filter);
            var paged = new PagingResponse<Employee>
            {
                Total = total,
                PageSize = pageSize,
                PageIndex = pageIndex,
                Data = data.ToList()
            };

            return CreateSuccessResponse(paged);
        }

        /// <summary>
        /// Sau validation UX, Insert DB có thể vẫn trùng (race) — repository ném <see cref="DuplicateEmployeeCodeException"/>; map ra message thân thiện.
        /// </summary>
        public override async Task<ServiceResponse> InsertAsync(Employee entity)
        {
            try
            {
                return await base.InsertAsync(entity);
            }
            catch (DuplicateEmployeeCodeException)
            {
                return CreateErrorResponse(ResponseCode.BadRequest, "Duplicate employee code", "Mã nhân viên đã tồn tại");
            }
        }

        /// <summary>
        /// Tương tự Insert khi đổi sang mã đã có (race hoặc bỏ qua validation).
        /// </summary>
        public override async Task<ServiceResponse> UpdateAsync(Guid entityId, Employee entity)
        {
            try
            {
                return await base.UpdateAsync(entityId, entity);
            }
            catch (DuplicateEmployeeCodeException)
            {
                return CreateErrorResponse(ResponseCode.BadRequest, "Duplicate employee code", "Mã nhân viên đã tồn tại");
            }
        }
    }
}