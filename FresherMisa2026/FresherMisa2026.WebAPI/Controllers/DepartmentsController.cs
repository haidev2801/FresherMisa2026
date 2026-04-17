using FresherMisa2026.Application.Interfaces;
using FresherMisa2026.Application.Interfaces.Services;
using FresherMisa2026.Application.Services;
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
        /// Láº¥y department theo code
        /// </summary>
        /// <returns></returns>
        /// Created By: dvhai (10/04/2026)
        [HttpGet("Code/{code}")]
        public async Task<ActionResult<ServiceResponse>> GetByCode(string code)
        {
            var response = new ServiceResponse();
            response.Data = await _departmentSerice.GetDepartmentByCodeAsync(code);
            response.IsSuccess = true;

            return response;
        }

        [HttpGet("{code}/employees")]
        public async Task<ActionResult<ServiceResponse>> GetEmployeesByDepartmentCode(string code)
        {
            var employees = await _departmentSerice.GetEmployeesByDepartmentCodeAsync(code);
            if (employees == null)
            {
                return NotFound(new ServiceResponse
                {
                    IsSuccess = false,
                    Code = (int)ResponseCode.NotFound,
                    DevMessage = "Không tìm thấy phòng ban",
                    UserMessage = "Không tìm thấy phòng ban"
                });
            }

            return Ok(new ServiceResponse
            {
                IsSuccess = true,
                Code = (int)ResponseCode.Success,
                Data = employees
            });
        }

        [HttpGet("{code}/employee-count")]
        public async Task<ActionResult<ServiceResponse>> GetEmployeeCountByDepartmentCode(string code)
        {
            var count = await _departmentSerice.GetEmployeeCountByDepartmentCodeAsync(code);
            if (count == null)
            {
                return NotFound(new ServiceResponse
                {
                    IsSuccess = false,
                    Code = (int)ResponseCode.NotFound,
                    DevMessage = "Không tìm thấy phòng ban",
                    UserMessage = "Không tìm thấy phòng ban"
                });
            }

            return Ok(new ServiceResponse
            {
                IsSuccess = true,
                Code = (int)ResponseCode.Success,
                Data = count
            });
        }
    }
}
