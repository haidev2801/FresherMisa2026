using FresherMisa2026.Application.Interfaces.Services;
using FresherMisa2026.Application.Services;
using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Employee;
using Microsoft.AspNetCore.Mvc;

namespace FresherMisa2026.WebAPI.Controllers
{
    [ApiController]
    public class EmployeesController : BaseController<Employee>
    {
        private readonly IEmployeeService _employeeService;
        public EmployeesController(IBaseService<Employee> baseService, IEmployeeService employeeService) : base(baseService)
        {
            _employeeService = employeeService;
        }

        [HttpGet("Filter")]
        public async Task<ActionResult<ServiceResponse>> GetFilter([FromQuery] FilterEmployeesRequest filterRequest)
        {
            var response = await _employeeService.GetEmployeeByFilter(filterRequest);
            return Ok(response);
        }
    }
}