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

        protected override List<ValidationError> ValidateCustom(Employee employee)
        {
            var errors = new List<ValidationError>();
            // Mã nhân viên không được trùng lặp
            var existingEmployee = _employeeRepository.GetEmployeeByCode(employee.EmployeeCode).Result;
                if (existingEmployee.EmployeeCode != employee.EmployeeCode && existingEmployee != null)
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