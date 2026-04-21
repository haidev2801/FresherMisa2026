using FresherMisa2026.Application.Interfaces;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Application.Interfaces.Services;
using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Employee;
using FresherMisa2026.Entities.Employee.DTO;
using FresherMisa2026.Entities.Enums;
using FresherMisa2026.Entities.Exceptions;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FresherMisa2026.Application.Services
{
    /// <summary>
    /// Nghiệp vụ xử lý nhân viên
    /// </summary>
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
        /// Lấy nhân viên theo mã
        /// </summary>
        /// <param name="code">Mã nhân viên</param>
        /// <returns>Nhân viên tìm thấy</returns>
        public async Task<Employee> GetEmployeeByCodeAsync(string code)
        {
            var employee = await _employeeRepository.GetEmployeeByEmployeeCodeAsync(code);
            if (employee == null)
                throw new Exception("Không tìm thấy employee");

            return employee;
        }

        /// <summary>
        /// Lọc danh sách nhân viên
        /// </summary>
        /// <param name="request">Điều kiện lọc</param>
        /// <returns>Kết quả phân trang nhân viên</returns>
        public async Task<ServiceResponse> GetFilter(FilterRequest request)
        {
            try
            {
                var employees = await _employeeRepository.GetFilter(request.DepartmentId, request.PositionId, request.SalaryFrom, request.SalaryTo, request.Gender, request.HireDateFrom, request.HireDateTo, request.PageSize ?? 20, request.PageIndex ?? 1);
                ////Xử lý paging
                //if (request.PageSize.HasValue && request.PageIndex.HasValue && request.PageSize.Value > 0 && request.PageIndex.Value > 0)
                //{
                //    var employeeList = employees.ToList();
                //    var totalRecords = employeeList.Count;
                //    var pageSize = request.PageSize.Value;
                //    var pageIndex = request.PageIndex.Value;
                //    var totalPages = totalRecords == 0 ? 0 : (int)Math.Ceiling((double)totalRecords / pageSize);

                //    var pagedData = employeeList
                //        .Skip((pageIndex - 1) * pageSize)
                //        .Take(pageSize)
                //        .ToList();

                //    var pagingResponse = new PagingResponse<Entities.Employee.Employee>
                //    {
                //        TotalRecords = totalRecords,
                //        TotalPages = totalPages,
                //        PageNumber = pageIndex,
                //        PageSize = pageSize,
                //        Data = pagedData
                //    };

                //    return CreateSuccessResponse(pagingResponse);
                //}
                var pagingResponse = new PagingResponse<Employee>
                {
                    Total = employees.TotalRecords,
                    TotalPages = employees.TotalRecords == 0 ? 0 : (int)Math.Ceiling((double)employees.TotalRecords / (request.PageSize ?? 20)),
                    PageNumber = request.PageIndex ?? 1,
                    PageSize = request.PageSize ?? 20,
                    Data = employees.employees.ToList()
                };

                return CreateSuccessResponse(ResponseCode.Success, pagingResponse);
            }
            catch (DatabaseException ex)
            {
                return CreateErrorResponse(Entities.Enums.ResponseCode.InternalServerError, "Lỗi lấy dữ liệu lọc nhân viên", ex.Message);
            }
        }

        /// <summary>
        /// Lấy danh sách nhân viên theo Id phòng ban
        /// </summary>
        /// <param name="departmentId">Id phòng ban</param>
        /// <returns>Danh sách nhân viên</returns>
        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentIdAsync(Guid departmentId)
        {
            return await _employeeRepository.GetEmployeesByDepartmentId(departmentId);
        }

        /// <summary>
        /// Lấy danh sách nhân viên theo Id vị trí
        /// </summary>
        /// <param name="positionId">Id vị trí</param>
        /// <returns>Danh sách nhân viên</returns>
        public async Task<IEnumerable<Employee>> GetEmployeesByPositionIdAsync(Guid positionId)
        {
            return await _employeeRepository.GetEmployeesByPositionId(positionId);
        }

        /// <summary>
        /// Validate tùy chỉnh cho Employee
        /// </summary>
        /// <param name="employee">Thông tin nhân viên</param>
        /// <returns>Danh sách lỗi</returns>
        protected override List<ValidationError> ValidateCustom(Entities.Employee.Employee employee)
        {
            var errors = new List<ValidationError>();
            //Kiểm tra trùng employee code
            var existedEmployeeCode = _employeeRepository.GetEmployeeByEmployeeCodeAsync(employee.EmployeeCode);
            if (existedEmployeeCode.Result != null)
            {
                errors.Add(new ValidationError("EmployeeCode", "Mã nhân viên đã tồn tại"));
            }

            if (!string.IsNullOrEmpty(employee.Email) && !Regex.IsMatch(employee.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase))
            {
                errors.Add(new ValidationError("EmployeeEmail", "Email không đúng định dạng"));
            }
            if (!string.IsNullOrEmpty(employee.PhoneNumber) && !Regex.IsMatch(employee.PhoneNumber, @"^(?:\+84|84|0)(3|5|7|8|9)([0-9]{8})$", RegexOptions.IgnoreCase))
            {
                errors.Add(new ValidationError("EmployeePhoneNumber", "Số điện thoại không hợp lệ"));
            }
            if (!string.IsNullOrEmpty(employee.EmployeeCode) && employee.EmployeeCode.Length > 20)
            {
                errors.Add(new ValidationError("EmployeeCode", "Mã nhân viên không được vượt quá 20 ký tự"));
            }

            if (string.IsNullOrEmpty(employee.EmployeeName))
            {
                errors.Add(new ValidationError("EmployeeName", "Tên nhân viên không được để trống"));
            }

            if (employee.DateOfBirth.HasValue && employee.DateOfBirth.Value.Date >= DateTime.Now.Date)
            {
                errors.Add(new ValidationError("EmployeeName", "Ngày sinh phải nhỏ hơn ngày hiện tại"));
            }

            return errors;
        }
    }
}