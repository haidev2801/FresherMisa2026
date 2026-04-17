using FresherMisa2026.Application.Interfaces;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Application.Interfaces.Services;
using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Employee;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;

namespace FresherMisa2026.Application.Services
{
    public class EmployeeService : BaseService<Employee>, IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        public EmployeeService(
            IBaseRepository<Employee> baseRepository,
            IEmployeeRepository employeeRepository,
            IMemoryCache cache
            ) : base(baseRepository, cache)
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

        public async Task<ServiceResponse> GetFilterAsync(FilterRequest filterRequest)
        {
            var response = new FilterResponse<Employee>();
            var rq = new FilterRequest
            {
                DepartmentId = filterRequest.DepartmentId == Guid.Empty ? null : filterRequest.DepartmentId,
                PositionId = filterRequest.PositionId == Guid.Empty ? null : filterRequest.PositionId,
                SalaryFrom = filterRequest.SalaryFrom,
                SalaryTo = filterRequest.SalaryTo,
                Gender = filterRequest.Gender,
                HireDateFrom = filterRequest.HireDateFrom,
                HireDateTo = filterRequest.HireDateTo,
                PageIndex = filterRequest.PageIndex > 0 ? filterRequest.PageIndex : 1,
                PageSize = filterRequest.PageSize > 0 ? filterRequest.PageSize : 10
            };

            response = await _employeeRepository.GetFilterAsync(rq);

            return CreateSuccessResponse(response);
        }
        protected override List<ValidationError> ValidateCustom(Employee employee)
        {
            var errors = new List<ValidationError>();
            //-Mã nhân viên không được trùng lặp
            var existingEmployee = _employeeRepository.GetEmployeeByCode(employee.EmployeeCode).Result;
            if(existingEmployee != null && existingEmployee.EmployeeID != employee.EmployeeID)
            {
                errors.Add(new ValidationError(
                    "EmployeeCode",
                    "Mã nhân viên đã tồn tại"
                ));
            }
            //-Email phải đúng định dạng(nếu có)
            if (!string.IsNullOrEmpty(employee.Email))
            {
                try
                {
                    var addr = new System.Net.Mail.MailAddress(employee.Email);
                }
                catch
                {
                    errors.Add(new ValidationError(
                        "Email",
                        "Email không đúng định dạng"
                    ));
                }
            }
            //-Số điện thoại phải đúng định dạng(nếu có)
            if (!string.IsNullOrEmpty(employee.PhoneNumber))
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(employee.PhoneNumber, @"^0\d{9,10}$"))
                {
                    errors.Add(new ValidationError(
                        "PhoneNumber",
                        "Số điện thoại không đúng định dạng"
                    ));
                }
            }
            //-Ngày sinh phải nhỏ hơn ngày hiện tại
            if (employee.DateOfBirth >= DateTime.Today)
            {
                errors.Add(new ValidationError(
                    "DateOfBirth",
                    "Ngày sinh phải nhỏ hơn ngày hiện tại"
                ));
            }

            return errors;
        }
    }
}