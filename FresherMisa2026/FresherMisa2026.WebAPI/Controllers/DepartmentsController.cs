using FresherMisa2026.Application.Interfaces.Services;
using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Department;
using FresherMisa2026.Entities.Enums;
using Microsoft.AspNetCore.Mvc;

namespace FresherMisa2026.WebAPI.Controllers
{
    [ApiController]
    public class DepartmentsController : BaseController<Department>
    {
        private readonly IDepartmentSerice _departmentSerice;

        public DepartmentsController(
            IDepartmentSerice departmentSerice) : base(departmentSerice)
        {
            _departmentSerice = departmentSerice;
        }


        /// <summary>
        /// Lấy department theo code
        /// </summary>
        /// <returns></returns>
        /// Created By: dvhai (10/04/2026)
        [HttpGet("Code/{code}")]
        public async Task<ActionResult<ServiceResponse>> GetByCode(string code)
        {
            var department = await _departmentSerice.GetDepartmentByCodeAsync(code);
            if (department == null)
            {
                return NotFound(CreateDepartmentNotFoundResponse(code));
            }

            return Ok(new ServiceResponse
            {
                IsSuccess = true,
                Code = (int)ResponseCode.Success,
                Data = department
            });
        }

        [HttpGet("{code}/employees")]
        public async Task<ActionResult<ServiceResponse>> GetEmployeesByDepartmentCode(string code)
        {
            var department = await _departmentSerice.GetDepartmentByCodeAsync(code);
            if (department == null)
            {
                return NotFound(CreateDepartmentNotFoundResponse(code));
            }

            var response = new ServiceResponse();
            response.Data = await _departmentSerice.GetEmployeesByDepartmentCodeAsync(code);
            response.IsSuccess = true;
            response.Code = (int)ResponseCode.Success;

            return Ok(response);
        }

        [HttpGet("{code}/employee-count")]
        public async Task<ActionResult<ServiceResponse>> GetEmployeeCountByDepartmentCode(string code)
        {
            var department = await _departmentSerice.GetDepartmentByCodeAsync(code);
            if (department == null)
            {
                return NotFound(CreateDepartmentNotFoundResponse(code));
            }

            var response = new ServiceResponse();
            var employees = await _departmentSerice.GetEmployeesByDepartmentCodeAsync(code);
            response.Data = employees.Count();
            response.IsSuccess = true;
            response.Code = (int)ResponseCode.Success;

            return Ok(response);
        }

        private static ServiceResponse CreateDepartmentNotFoundResponse(string code)
        {
            return new ServiceResponse
            {
                IsSuccess = false,
                Code = (int)ResponseCode.NotFound,
                UserMessage = "Không tìm thấy phòng ban",
                DevMessage = $"Không tìm thấy phòng ban có mã '{code}'"
            };
        }
    }
}
