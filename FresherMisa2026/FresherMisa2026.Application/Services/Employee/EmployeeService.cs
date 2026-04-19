using FresherMisa2026.Application.Interfaces;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Application.Interfaces.Services;
using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Employee;
using FresherMisa2026.Entities.Employee.DTO;
using FresherMisa2026.Entities.Enums;
using MySqlConnector;
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


        /// <summary>
        /// Lọc và phân trang danh sách nhân viên theo các tiêu chí: 
        /// phòng ban, chức vụ, mức lương, giới tính, ngày tuyển dụng
        /// </summary>
        /// <param name="filterRequest"></param>
        /// <returns></returns>
        /// Create By: Tannn (18/04/2026)
        public async Task<ServiceResponse> GetFilterEmployeesAsync(FilterRequest filterRequest)
        {
            var (total, employees) = await _employeeRepository.GetFilterEmployeesAsync(
                filterRequest.PageSize,
                filterRequest.PageIndex,
                filterRequest.DepartmentId,
                filterRequest.PositionId,
                filterRequest.SalaryFrom,
                filterRequest.SalaryTo,
                filterRequest.Gender,
                filterRequest.HireDateFrom,
                filterRequest.HireDateTo);

            var response = new PagingResponse<Employee>
            {
                Total = total,
                Data = employees.ToList(),
                PageSize = filterRequest.PageSize,
                PageIndex = filterRequest.PageIndex
            };

            return CreateSuccessResponse(response);
        }

        protected override List<ValidationError> ValidateCustom(Employee employee)
        {
            var errors = new List<ValidationError>();

            //if (!string.IsNullOrEmpty(employee.EmployeeCode))
            //{
            //    var existingEmployee = _employeeRepository.GetEmployeeByCode(employee.EmployeeCode).Result;
            //    if (existingEmployee != null)
            //    {
            //        errors.Add(new ValidationError("EmployeeCode", "Mã nhân viên đã tồn tại"));
            //    }
            //}

            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!string.IsNullOrEmpty(employee.Email) && !Regex.IsMatch(employee.Email, emailPattern))
            {
                errors.Add(new ValidationError("Email", "Email không hợp lệ"));
            }

            string phonePattern = @"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$";
            if (!string.IsNullOrEmpty(employee.PhoneNumber) && !Regex.IsMatch(employee.PhoneNumber, phonePattern))
            {
                errors.Add(new ValidationError("Phone", "Số điện thoại không hợp lệ"));
            }

            if (employee.DateOfBirth != null && employee.DateOfBirth > DateTime.Now)
            {
                errors.Add(new ValidationError("DateOfBirth", "Ngày sinh không được lớn hơn ngày hiện tại"));
            }

            return errors;
        }

        public override async Task<ServiceResponse> InsertAsync(Employee employee)
        {
            try
            {
                return await base.InsertAsync(employee);
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                return CreateErrorResponse(ResponseCode.BadRequest, "Mã nhân viên đã tồn tại");
            }
        }
    }
}