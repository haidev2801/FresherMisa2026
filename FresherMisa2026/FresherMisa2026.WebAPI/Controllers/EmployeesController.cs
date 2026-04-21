using FresherMisa2026.Application.Interfaces.Services;
using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Employee;
using FresherMisa2026.Entities.Employee.DTO;
using Microsoft.AspNetCore.Mvc;

namespace FresherMisa2026.WebAPI.Controllers
{
    [ApiController]
    /// <summary>
    /// API quản lý nhân viên
    /// </summary>
    public class EmployeesController : BaseController<Employee>
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(
            IEmployeeService employeeService) : base(employeeService)
        {
            _employeeService = employeeService;
        }

        /// <summary>
        /// Lấy nhân viên theo mã
        /// </summary>
        /// <param name="code">Mã nhân viên</param>
        /// <returns>Thông tin nhân viên</returns>
        [HttpGet("Code/{code}")]
        public async Task<ActionResult<ServiceResponse>> GetByCode(string code)
        {
            var response = new ServiceResponse();
            response.Data = await _employeeService.GetEmployeeByCodeAsync(code);
            response.IsSuccess = true;

            return response;
        }

        /// <summary>
        /// Lấy danh sách nhân viên theo Id phòng ban
        /// </summary>
        /// <param name="departmentId">Id phòng ban</param>
        /// <returns>Danh sách nhân viên</returns>
        [HttpGet("Department/{departmentId}")]
        public async Task<ActionResult<ServiceResponse>> GetByDepartmentId(Guid departmentId)
        {
            var response = new ServiceResponse();
            response.Data = await _employeeService.GetEmployeesByDepartmentIdAsync(departmentId);
            response.IsSuccess = true;

            return response;
        }

        /// <summary>
        /// Lấy danh sách nhân viên theo Id vị trí
        /// </summary>
        /// <param name="positionId">Id vị trí</param>
        /// <returns>Danh sách nhân viên</returns>
        [HttpGet("Position/{positionId}")]
        public async Task<ActionResult<ServiceResponse>> GetByPositionId(Guid positionId)
        {
            var response = new ServiceResponse();
            response.Data = await _employeeService.GetEmployeesByPositionIdAsync(positionId);
            response.IsSuccess = true;

            return response;
        }

        /// <summary>
        /// Lọc danh sách nhân viên
        /// </summary>
        /// <returns>Kết quả lọc nhân viên</returns>
        [HttpGet("filter")]
        public async Task<ActionResult<ServiceResponse>> Filter(
            string? departmentId,
            string? positionId,
            decimal? salaryFrom,
            decimal? salaryTo,
            int? gender,
            DateTime? hireDateFrom,
            DateTime? hireDateTo,
            int? pageSize,
            int? pageIndex
            )
        {
            var request = new FilterRequest
            {
                DepartmentId = departmentId,
                PositionId = positionId,
                SalaryFrom = salaryFrom,
                SalaryTo = salaryTo,
                Gender = gender,
                HireDateFrom = hireDateFrom,
                HireDateTo = hireDateTo,
                PageSize = pageSize,
                PageIndex = pageIndex
            };
            var response = await _employeeService.GetFilter(request);
            return Ok(response);
        }
    }
}