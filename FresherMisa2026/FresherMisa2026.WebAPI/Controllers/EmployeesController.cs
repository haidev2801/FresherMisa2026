using FresherMisa2026.Application.Interfaces.Services;
using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Employee;
using FresherMisa2026.Entities.Enums;
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
            try
            {
                var response = new ServiceResponse();
                response.Data = await _employeeService.GetEmployeeByCodeAsync(code);
                response.IsSuccess = true;

                return response;
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ServiceResponse
                {
                    IsSuccess = false,
                    Code = (int)ResponseCode.NotFound,
                    DevMessage = ex.Message
                });
            }
        }

        [HttpGet("Department/{departmentId}")]
        public async Task<ActionResult<ServiceResponse>> GetByDepartmentId(Guid departmentId)
        {
            var response = new ServiceResponse
            {
                IsSuccess = true,
                Code = (int)ResponseCode.Success,
                Data = await _employeeService.GetEmployeesByDepartmentIdAsync(departmentId)
            };

            return response;
        }

        [HttpGet("Position/{positionId}")]
        public async Task<ActionResult<ServiceResponse>> GetByPositionId(Guid positionId)
        {
            var response = new ServiceResponse
            {
                IsSuccess = true,
                Code = (int)ResponseCode.Success,
                Data = await _employeeService.GetEmployeesByPositionIdAsync(positionId)
            };

            return response;
        }

        [HttpGet("filter")]
        public async Task<ActionResult<ServiceResponse>> Filter([FromQuery] EmployeeFilterRequest request)
        {
            var response = await _employeeService.GetEmployeesByFilterAsync(request);

            if (!response.IsSuccess && response.Code == (int)ResponseCode.BadRequest)
                return BadRequest(response);

            return Ok(response);
        }
    }
}