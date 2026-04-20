using FresherMisa2026.Application.Interfaces;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Application.Interfaces.Services;
using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Employee;
using FresherMisa2026.Entities.Enums;
using System.Linq;
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

        /// <summary>
        /// Lấy employee bằng code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        /// Created by: Phuong (17/04/2026)
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

        /// <summary>
        /// Lọc nhân viên theo nhiều điều kiện
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="positionId"></param>
        /// <param name="salaryFrom"></param>
        /// <param name="salaryTo"></param>
        /// <param name="gender"></param>
        /// <param name="hireDateFrom"></param>
        /// <param name="hireDateTo"></param>
        /// <returns></returns>
        /// Created by: Phuong (18/04/2026)
        public async Task<ServiceResponse> GetEmployeesFilterAsync(
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
            var (total, data) = await _employeeRepository.GetEmployeesFilterAsync(
                departmentId, 
                positionId, 
                salaryFrom, 
                salaryTo, 
                gender, 
                hireDateFrom, 
                hireDateTo,
                pageSize,
                pageIndex);

            var response = new PagingResponse<Employee>
            {
                Total = total,
                Data = data.ToList(),
                PageSize = pageSize,
                PageIndex = pageIndex
            };

            return CreateSuccessResponse(response);
        }

        public override async Task<ServiceResponse> InsertAsync(Employee entity)
        {
            try
            {
                return await base.InsertAsync(entity);
            }
            catch (Exception ex) when (ex.Message.Contains("tồn tại"))
            {
                return CreateErrorResponse(ResponseCode.BadRequest, "Mã nhân viên đã tồn tại", ex.Message);
            }
        }

        public override async Task<ServiceResponse> UpdateAsync(Guid entityId, Employee entity)
        {
            try
            {
                return await base.UpdateAsync(entityId, entity);
            }
            catch (Exception ex) when (ex.Message.Contains("tồn tại"))
            {
                return CreateErrorResponse(ResponseCode.BadRequest, "Mã nhân viên đã tồn tại", ex.Message);
            }
        }

        /// <summary>
        /// Validate employee
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        /// Created by: Phuong (18/04/2026)
        protected override List<ValidationError> ValidateCustom(Employee employee)
        {
            var errors = new List<ValidationError>();

            // 1. Kiểm tra mã nhân viên không được vượt quá 20 ký tự
            if (!string.IsNullOrEmpty(employee.EmployeeCode) && employee.EmployeeCode.Length > 20)
            {
                errors.Add(new ValidationError("EmployeeCode", "Mã nhân viên không được vượt quá 20 ký tự"));
            }

            // 2. Kiểm tra mã nhân viên không được trùng lặp
            if (!string.IsNullOrEmpty(employee.EmployeeCode))
            {
                var existingEmployee = _employeeRepository.GetEmployeeByCode(employee.EmployeeCode).GetAwaiter().GetResult();
                if (existingEmployee != null)
                {
                    // Nếu là thêm mới, hoặc là cập nhật nhưng ID khác với bản ghi hiện có -> Trùng mã
                    if (employee.State == ModelState.Add || (employee.State == ModelState.Update && existingEmployee.EmployeeID != employee.EmployeeID))
                    {
                        errors.Add(new ValidationError("EmployeeCode", "Mã nhân viên đã tồn tại trong hệ thống"));
                    }
                }
            }

            // 3. Email đúng định dạng (nếu có)
            if (!string.IsNullOrEmpty(employee.Email) && !Regex.IsMatch(employee.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                errors.Add(new ValidationError("Email", "Email không đúng định dạng"));
            }

            // 4. Số điện thoại đúng định dạng (nếu có - kiểm tra chỉ chứa số, độ dài 10-11 ký tự)
            if (!string.IsNullOrEmpty(employee.PhoneNumber) && !Regex.IsMatch(employee.PhoneNumber, @"^[0-9]{10,11}$"))
            {
                errors.Add(new ValidationError("PhoneNumber", "Số điện thoại không đúng định dạng"));
            }

            // 5. Ngày sinh phải nhỏ hơn ngày hiện tại
            if (employee.DateOfBirth.HasValue && employee.DateOfBirth.Value >= DateTime.Now)
            {
                errors.Add(new ValidationError("DateOfBirth", "Ngày sinh phải nhỏ hơn ngày hiện tại"));
            }

            return errors;
        }
    }
}