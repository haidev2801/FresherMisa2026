using FresherMisa2026.Application.Dtos.Employee;
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

        [HttpGet("filter")]
        public async Task<IActionResult> GetEmployeeFilter([FromQuery] GetEmployeeFilterDto dto)
        {
            return Ok(await _employeeService.GetEmployeeFilterAsync(dto));
        }

        [HttpPut("update-dto")]
        public async Task<IActionResult> UpdateDto(Guid id, UpdateEmployeeDto dto)
        {
            var response = await _employeeService.UpdateDtoAsync(id, dto);

            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpPost("create-dto")]
        public async Task<IActionResult> CreateDto(CreateEmployeeDto dto)
        {
            var response = await _employeeService.CreateDtoAsync(dto);

            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }
    }
}