using FresherMisa2026.Application.Interfaces.Services;
using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Employee;
using Microsoft.AspNetCore.Mvc;

namespace FresherMisa2026.WebAPI.Controllers
{
    [ApiController]
    public class EmployeesController : BaseController<Employee>
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(
            IEmployeeService employeeService) : base(employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet("Code/{code}")]
        public async Task<ActionResult<ServiceResponse>> GetByCode(string code)
        {
            var response = new ServiceResponse();
            response.Data = await _employeeService.GetEmployeeByCodeAsync(code);
            response.IsSuccess = true;

            return response;
        }

        [HttpGet("Department/{departmentId}")]
        public async Task<ActionResult<ServiceResponse>> GetByDepartmentId(Guid departmentId)
        {
            var response = new ServiceResponse();
            response.Data = await _employeeService.GetEmployeesByDepartmentIdAsync(departmentId);
            response.IsSuccess = true;

            return response;
        }

        [HttpGet("Position/{positionId}")]
        public async Task<ActionResult<ServiceResponse>> GetByPositionId(Guid positionId)
        {
            var response = new ServiceResponse();
            response.Data = await _employeeService.GetEmployeesByPositionIdAsync(positionId);
            response.IsSuccess = true;

            return response;
        }

        /// <summary>
        /// Task 2.2: Lọc nhân viên theo nhiều điều kiện
        /// </summary>
        [HttpGet("filter")]
        public async Task<ActionResult<ServiceResponse>> Filter(
            [FromQuery] Guid? departmentId,
            [FromQuery] Guid? positionId,
            [FromQuery] decimal? salaryFrom,
            [FromQuery] decimal? salaryTo,
            [FromQuery] int? gender,
            [FromQuery] DateTime? hireDateFrom,
            [FromQuery] DateTime? hireDateTo)
        {
            var response = new ServiceResponse();
            response.Data = await _employeeService.FilterEmployeesAsync(
                departmentId, positionId, salaryFrom, salaryTo, gender, hireDateFrom, hireDateTo);
            response.IsSuccess = true;

            return Ok(response);
        }

        /// <summary>
        /// Task 3.3: Lọc nhân viên có phân trang
        /// </summary>
        [HttpGet("filter-paging")]
        public async Task<ActionResult<ServiceResponse>> FilterPaging(
            [FromQuery] Guid? departmentId,
            [FromQuery] Guid? positionId,
            [FromQuery] decimal? salaryFrom,
            [FromQuery] decimal? salaryTo,
            [FromQuery] int? gender,
            [FromQuery] DateTime? hireDateFrom,
            [FromQuery] DateTime? hireDateTo,
            [FromQuery] int pageSize = 10,
            [FromQuery] int pageIndex = 1)
        {
            var response = new ServiceResponse();
            response.Data = await _employeeService.FilterEmployeesWithPagingAsync(
                departmentId, positionId, salaryFrom, salaryTo, gender, hireDateFrom, hireDateTo,
                pageSize, pageIndex);
            response.IsSuccess = true;

            return Ok(response);
        }
    }
}