using FresherMisa2026.Application.Interfaces.Services;
using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Department;
using Microsoft.AspNetCore.Mvc;

namespace FresherMisa2026.WebAPI.Controllers
{
    [ApiController]
    public class DepartmentsController : BaseController<Department>
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentsController(IDepartmentService departmentSerice) : base(departmentSerice)
        {
            _departmentService = departmentSerice;
        }


        /// <summary>
        /// Lấy department theo code
        /// </summary>
        /// <returns></returns>
        /// Created By: dvhai (10/04/2026)
        [HttpGet("Code/{code}")]
        public async Task<ActionResult<ServiceResponse>> GetByCode(string code)
        {
            var response = new ServiceResponse();
            response.Data = await _departmentService.GetDepartmentByCodeAsync(code);
            response.IsSuccess = true;

            return response;
        }

        /// <summary>
        /// Lấy danh sách nhân viên theo mã phòng ban
        /// </summary>
        [HttpGet("{code}/employees")]
        public async Task<ActionResult<ServiceResponse>> GetEmployeesByDepartmentCode(string code)
        {
            var response = new ServiceResponse
            {
                Data = await _departmentService.GetEmployeesByDepartmentCodeAsync(code),
                IsSuccess = true
            };

            return Ok(response);
        }

        /// <summary>
        /// Đếm số lượng nhân viên theo mã phòng ban
        /// </summary>
        [HttpGet("{code}/employee-count")]
        public async Task<ActionResult<ServiceResponse>> GetEmployeeCountByDepartmentCode(string code)
        {
            var response = new ServiceResponse
            {
                Data = await _departmentService.GetEmployeeCountByDepartmentCodeAsync(code),
                IsSuccess = true
            };

            return Ok(response);
        }
    }
}
