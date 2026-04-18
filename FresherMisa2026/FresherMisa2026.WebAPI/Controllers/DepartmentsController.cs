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
            var response = new ServiceResponse();
            response.Data = await _departmentSerice.GetDepartmentByCodeAsync(code);
            response.IsSuccess = true;

            return response;
        }

        /// <summary>
        /// Lấy danh sách nhân viên theo department code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        /// Created By: tannn (18/04/2026)
        [HttpGet("{code}/employees")]
        public async Task<ActionResult<ServiceResponse>> GetEmployeesByCode(string code)
        {
            var response = await _departmentSerice.GetEmployeesByDepartmentCodeAsync(code);

            return Ok(response);
        }

        /// <summary>
        /// Lấy số lượng nhân viên theo department code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        /// Created By: tannn (18/04/2026)
        [HttpGet("{code}/employee-count")]
        public async Task<ActionResult<ServiceResponse>> GetEmployeeCount(string code)
        {
            var response = await _departmentSerice.CountEmployeesByDepartmentCodeAsync(code);

            return Ok(response);
        }
    }
}
