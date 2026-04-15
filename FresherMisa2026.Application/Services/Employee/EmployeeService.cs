using FresherMisa2026.Application.Interfaces;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Application.Interfaces.Services;
using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Employee;
using System;
using System.Collections.Generic;
using System.Text;

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
        /// Lấy employee theo code
        /// </summary>
        /// <param name="code">Mã employee</param>
        /// <returns>Employee tìm thấy</returns>
        /// Created By: dvhai (15/04/2026)
        public async Task<Employee> GetEmployeeByCodeAsync(string code)
        {
            var employee = await _employeeRepository.GetEmployeeByCode(code);
            if (employee == null)
                throw new Exception("Employee not found");

            return employee;
        }

        /// <summary>
        /// Lấy danh sách employee theo department id
        /// </summary>
        /// <param name="departmentId">ID phòng ban</param>
        /// <returns>Danh sách employee</returns>
        /// Created By: dvhai (15/04/2026)
        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentIdAsync(Guid departmentId)
        {
            return await _employeeRepository.GetEmployeesByDepartmentId(departmentId);
        }

        /// <summary>
        /// Lấy danh sách employee theo position id
        /// </summary>
        /// <param name="positionId">ID vị trí</param>
        /// <returns>Danh sách employee</returns>
        /// Created By: dvhai (15/04/2026)
        public async Task<IEnumerable<Employee>> GetEmployeesByPositionIdAsync(Guid positionId)
        {
            return await _employeeRepository.GetEmployeesByPositionId(positionId);
        }

        #region OVERRIDE METHODS

        /// <summary>
        /// Validate tùy chỉnh cho Employee
        /// </summary>
        protected override List<ValidationError> ValidateCustom(Employee employee)
        {
            var errors = new List<ValidationError>();

            // Ví dụ: Kiểm tra mã nhân viên không được vượt quá 20 ký tự
            if (!string.IsNullOrEmpty(employee.EmployeeCode) && employee.EmployeeCode.Length > 20)
            {
                errors.Add(new ValidationError("EmployeeCode", "Mã nhân viên không được vượt quá 20 ký tự"));
            }

            // Kiểm tra tên nhân viên không được để trống
            if (string.IsNullOrEmpty(employee.EmployeeName))
            {
                errors.Add(new ValidationError("EmployeeName", "Tên nhân viên không được để trống"));
            }

            return errors;
        }

        #endregion OVERRIDE METHODS
    }
}