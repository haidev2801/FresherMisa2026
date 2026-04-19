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
        private readonly IEmployeeService _employeeService;

        public DepartmentsController(
            IDepartmentSerice departmentSerice,
            IEmployeeService employeeService) : base(departmentSerice)
        {
            _departmentSerice = departmentSerice;
            _employeeService = employeeService;
        }


        /// <summary>
        /// Lấy department theo code
        /// </summary>
        /// <returns></returns>
        /// Created By: PXCUONG (10/04/2026)
        [HttpGet("Code/{code}")]
        public async Task<ActionResult<ServiceResponse>> GetByCode(string code)
        {
            var response = new ServiceResponse();
            response.Data = await _departmentSerice.GetDepartmentByCodeAsync(code);
            response.IsSuccess = true;
            response.Code = (int)ResponseCode.Success;

            return response;
        }

        /// <summary>
        /// Lấy danh sách nhân viên theo mã phòng ban
        /// </summary>
        /// <param name="code">Mã phòng ban</param>
        /// <returns>Danh sách nhân viên</returns>
        [HttpGet("{code}/employees")]
        public async Task<ActionResult<ServiceResponse>> GetEmployeesByDepartmentCode(string code)
        {
            var response = new ServiceResponse();
            response.Data = await _employeeService.GetEmployeesByDepartmentCodeAsync(code);
            response.IsSuccess = true;
            response.Code = (int)ResponseCode.Success;

            return Ok(response);
        }

        /// <summary>
        /// Đếm số lượng nhân viên theo mã phòng ban
        /// </summary>
        /// <param name="code">Mã phòng ban</param>
        /// <returns>Số lượng nhân viên</returns>
        [HttpGet("{code}/employee-count")]
        public async Task<ActionResult<ServiceResponse>> CountEmployeesByDepartmentCode(string code)
        {
            var response = new ServiceResponse();
            response.Data = await _employeeService.CountEmployeesByDepartmentCodeAsync(code);
            response.IsSuccess = true;
            response.Code = (int)ResponseCode.Success;

            return Ok(response);
        }
    }
}
