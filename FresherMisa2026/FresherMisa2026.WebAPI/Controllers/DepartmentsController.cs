using FresherMisa2026.Application.Interfaces;
using FresherMisa2026.Application.Interfaces.Services;
using FresherMisa2026.Application.Services;
using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Department;
using Microsoft.AspNetCore.Mvc;

namespace FresherMisa2026.WebAPI.Controllers
{
    [ApiController]
    /// <summary>
    /// API quản lý phòng ban
    /// </summary>
    public class DepartmentsController : BaseController<Department>
    {
        private readonly IDepartmentSerice _departmentService;

        public DepartmentsController(
            IDepartmentSerice departmentService) : base(departmentService)
        {
            _departmentService = departmentService;
        }


        /// <summary>
        /// Lấy department theo code
        /// </summary>
        /// <param name="code">Mã phòng ban</param>
        /// <returns>Thông tin phòng ban</returns>
        /// Created By: dvhai (10/04/2026)
        [HttpGet("Code/{code}")]
        public async Task<ActionResult<ServiceResponse>> GetByCode(string? code)
        {
            var response = await _departmentService.GetDepartmentByCodeAsync(code);
            return Ok(response);
        }

        /// <summary>
        /// Lấy danh sách nhân viên theo mã phòng ban
        /// </summary>
        /// <param name="code">Mã phòng ban</param>
        /// <returns>Danh sách nhân viên</returns>
        [HttpGet("{code}/employees")]
        public async Task<ActionResult<ServiceResponse>> GetEmployeesByDepartmentCode(string? code)
        {
            var response = await _departmentService.GetEmployeesByDepartmentCode(code);
            return Ok(response);
        }

        /// <summary>
        /// Lấy số lượng nhân viên theo mã phòng ban
        /// </summary>
        /// <param name="code">Mã phòng ban</param>
        /// <returns>Số lượng nhân viên</returns>
        [HttpGet("{code}/employee-count")]
        public async Task<ActionResult<ServiceResponse>> GetEmployeeCountByDepartmentCode(string? code)
        {
            var response = await _departmentService.GetEmployeeCountByDepartmentCode(code);
            return Ok(response);
        }
    }
}
