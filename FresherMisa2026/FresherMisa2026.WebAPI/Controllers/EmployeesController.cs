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
        public async Task<ActionResult<ServiceResponse>> Filter(
            [FromQuery] Guid? departmentId,
            [FromQuery] Guid? positionId,
            [FromQuery] decimal? salaryFrom,
            [FromQuery] decimal? salaryTo,
            [FromQuery] int? gender,
            [FromQuery] DateTime? hireDateFrom,
            [FromQuery] DateTime? hireDateTo
        )
        {
            if (gender.HasValue && (gender.Value < 0 || gender.Value > 2))
            {
                return BadRequest(new ServiceResponse
                {
                    IsSuccess = false,
                    Code = 400,
                    DevMessage = "gender không hợp lệ. Chỉ chấp nhận 0, 1, 2."
                });
            }

            if (salaryFrom.HasValue && salaryTo.HasValue && salaryFrom > salaryTo)
            {
                return BadRequest(new ServiceResponse
                {
                    IsSuccess = false,
                    Code = 400,
                    DevMessage = "salaryFrom không được lớn hơn salaryTo."
                });
            }

            if (hireDateFrom.HasValue && hireDateTo.HasValue && hireDateFrom > hireDateTo)
            {
                return BadRequest(new ServiceResponse
                {
                    IsSuccess = false,
                    Code = 400,
                    DevMessage = "hireDateFrom không được lớn hơn hireDateTo."
                });
            }

            var request = new EmployeeFilterRequest
            {
                DepartmentId = departmentId,
                PositionId = positionId,
                SalaryFrom = salaryFrom,
                SalaryTo = salaryTo,
                Gender = gender,
                HireDateFrom = hireDateFrom,
                HireDateTo = hireDateTo
            };

            var response = new ServiceResponse
            {
                Data = await _employeeService.FilterEmployeesAsync(request),
                IsSuccess = true
            };

            return Ok(response);
        }
    }
}
