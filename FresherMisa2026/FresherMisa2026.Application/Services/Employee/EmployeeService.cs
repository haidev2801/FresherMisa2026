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


        // Lọc nhân viên theo nhiều tiêu chí
        // Nếu request.DepartmentId = Guid.Empty thì không lọc theo phòng ban
        // Nếu request.PositionId = Guid.Empty thì không lọc theo chức vụ
        // author: Dieuhoang 21/04/2026
        public async Task<ServiceResponse> GetEmployeeByFilter(FilterEmployeeRq request)
        {
            var response = new FilterResponse<Employee>();

            var rq = new FilterEmployeeRq
            {
                DepartmentID = request.DepartmentID == Guid.Empty ? null : request.DepartmentID,
                PositionID = request.PositionID == Guid.Empty ? null : request.PositionID,
                SalaryFrom = request.SalaryFrom,
                SalaryTo = request.SalaryTo,
                Gender = request.Gender,
                HireDateFrom = request.HireDateFrom,
                HireDateTo = request.HireDateTo,
            };

            response = await _employeeRepository.GetEmployeesByFilter(rq);

            return CreateSuccessResponse(response);
        }


        protected override async Task<List<ValidationError>> ValidateCustomAsync(Employee entity)
        {
            var errors = new List<ValidationError>();

            // 1. Trùng mã
            var existed = await _employeeRepository.GetEmployeeByCode(entity.EmployeeCode);
            if (existed != null && existed.EmployeeID != entity.EmployeeID)
            {
                errors.Add(new ValidationError("EmployeeCode", "Mã nhân viên đã tồn tại"));
            }

            // 2. Email
            if (!string.IsNullOrEmpty(entity.Email))
            {
                var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                if (!regex.IsMatch(entity.Email))
                {
                    errors.Add(new ValidationError("Email", "Email không đúng định dạng"));
                }
            }

            // 3. Phone
            if (!string.IsNullOrEmpty(entity.PhoneNumber))
            {
                var regex = new Regex(@"^(0|\+84)[0-9]{9,10}$");
                if (!regex.IsMatch(entity.PhoneNumber))
                {
                    errors.Add(new ValidationError("PhoneNumber", "Số điện thoại không đúng định dạng"));
                }
            }

            // 4. DOB
            if (entity.DateOfBirth.HasValue && entity.DateOfBirth >= DateTime.Now)
            {
                errors.Add(new ValidationError("DateOfBirth", "Ngày sinh phải nhỏ hơn ngày hiện tại"));
            }

            return errors;
        }
    }
}