using FresherMisa2026.Application.Interfaces.Services;
using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Employee;
using FresherMisa2026.Entities.Employee.DTO;
using Microsoft.AspNetCore.Mvc;

namespace FresherMisa2026.WebAPI.Controllers
{
    /// <summary>
    /// API quản lý nhân viên.
    /// Các API CRUD cơ bản được kế thừa từ BaseController:
    /// - GET /api/Employees
    /// - GET /api/Employees/{id}
    /// - POST /api/Employees
    /// - PUT /api/Employees/{id}
    /// - DELETE /api/Employees/{id}
    /// </summary>
    [ApiController]
    public class EmployeesController : BaseController<Employee>
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(
            IEmployeeService employeeService) : base(employeeService)
        {
            _employeeService = employeeService;
        }

        /// <summary>
        /// Lấy nhân viên theo mã nhân viên.
        /// </summary>
        /// <param name="code">Mã nhân viên</param>
        /// <returns>Thông tin nhân viên tương ứng</returns>
        [HttpGet("Code/{code}")]
        public async Task<ActionResult<ServiceResponse>> GetByCode(string code)
        {
            var response = new ServiceResponse();
            response.Data = await _employeeService.GetEmployeeByCodeAsync(code);
            response.IsSuccess = true;

            return response;
        }

        /// <summary>
        /// Lấy danh sách nhân viên theo phòng ban.
        /// </summary>
        /// <param name="departmentId">Id phòng ban</param>
        /// <returns>Danh sách nhân viên thuộc phòng ban</returns>
        [HttpGet("Department/{departmentId}")]
        public async Task<ActionResult<ServiceResponse>> GetByDepartmentId(Guid departmentId)
        {
            var response = new ServiceResponse();
            response.Data = await _employeeService.GetEmployeesByDepartmentIdAsync(departmentId);
            response.IsSuccess = true;

            return response;
        }

        /// <summary>
        /// Lấy danh sách nhân viên theo vị trí.
        /// </summary>
        /// <param name="positionId">Id vị trí</param>
        /// <returns>Danh sách nhân viên thuộc vị trí</returns>
        [HttpGet("Position/{positionId}")]
        public async Task<ActionResult<ServiceResponse>> GetByPositionId(Guid positionId)
        {
            var response = new ServiceResponse();
            response.Data = await _employeeService.GetEmployeesByPositionIdAsync(positionId);
            response.IsSuccess = true;

            return response;
        }

        /// <summary>
        /// Lọc danh sách nhân viên theo nhiều điều kiện.
        /// </summary>
        /// <param name="filterRequest">Thông tin điều kiện lọc</param>
        /// <returns>Danh sách nhân viên phù hợp điều kiện</returns>
        [HttpGet("filter")]
        public async Task<ActionResult<ServiceResponse>> Filter([FromQuery] EmployeeFilterRequest filterRequest)
        {
            var response = new ServiceResponse
            {
                Data = await _employeeService.FilterEmployeesAsync(filterRequest),
                IsSuccess = true
            };

            return Ok(response);
        }
    }
}