using FresherMisa2026.Application.Interfaces;
using FresherMisa2026.Application.Interfaces.Services;
using FresherMisa2026.Application.Services;
using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Department;
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
        /// Lấy danh sách nhân viên theo mã phòng ban
        /// Modified by: ChucTC1 - 21/4/2026
        /// Sửa mã lỗi 500 thành 404 khi không tìm thấy phòng ban theo mã
        /// </summary>
        //[HttpGet("{code}/employees")]
        //public async Task<ActionResult<ServiceResponse>> GetEmployeesByDepartmentCode(string code)
        //{
        //    var response = new ServiceResponse();
        //    response.Data = await _departmentSerice.GetEmployeesByDepartmentCodeAsync(code);
        //    response.IsSuccess = true;

        //    return response;
        //}
        [HttpGet("{code}/employees")]
        public async Task<ActionResult<ServiceResponse>> GetEmployeesByDepartmentCode(string code)
        {
            var department = await _departmentSerice.GetDepartmentByCodeAsync(code);
            if (department == null)
            {
                return NotFound(new ServiceResponse
                {
                    IsSuccess = false,
                    Code = 404,
                    DevMessage = $"Không tìm thấy phòng ban có mã: {code}"
                });
            }

            var response = new ServiceResponse();
            response.Data = await _departmentSerice.GetEmployeesByDepartmentCodeAsync(code);
            response.IsSuccess = true;
            return Ok(response);
        }

        /// <summary>
        /// Đếm số nhân viên trong phòng ban
        /// Modified by: ChucTC1 - 21/4/2026
        /// Sửa mã throw 500 
        /// </summary>
        //[HttpGet("{code}/employee-count")]
        //public async Task<ActionResult<ServiceResponse>> GetEmployeeCountByDepartmentCode(string code)
        //{
        //    var response = new ServiceResponse();
        //    response.Data = await _departmentSerice.GetEmployeeCountByDepartmentCodeAsync(code);
        //    response.IsSuccess = true;

        //    return response;
        //}
        [HttpGet("{code}/employee-count")]
        public async Task<ActionResult<ServiceResponse>> GetEmployeeCountByDepartmentCode(string code)
        {
            var department = await _departmentSerice.GetDepartmentByCodeAsync(code);
            if (department == null)
            {
                return NotFound(new ServiceResponse
                {
                    IsSuccess = false,
                    Code = 404,
                    DevMessage = $"Không tìm thấy phòng ban có mã: {code}"
                });
            }

            var response = new ServiceResponse();
            response.Data = await _departmentSerice.GetEmployeeCountByDepartmentCodeAsync(code);
            response.IsSuccess = true;
            return Ok(response);
        }
    }
}
