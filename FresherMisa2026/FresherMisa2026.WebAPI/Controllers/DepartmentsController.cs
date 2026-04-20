using FresherMisa2026.Application.Interfaces;
using FresherMisa2026.Application.Interfaces.Services;
using FresherMisa2026.Application.Services;
using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Department;
using Microsoft.AspNetCore.Mvc;
using FresherMisa2026.Entities.Enums;
using System.Linq;

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
            var response = new ServiceResponse();
            try
            {
                var department = await _departmentSerice.GetDepartmentByCodeAsync(code);
                if (department == null)
                {
                    response.IsSuccess = false;
                    response.Code = (int)ResponseCode.NotFound;
                    response.UserMessage = "Không tìm thấy phòng ban";
                    return NotFound(response);
                }

                var employees = await _employeeService.GetEmployeesByDepartmentIdAsync(department.DepartmentID);
                response.Data = employees;
                response.IsSuccess = true;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = (int)ResponseCode.InternalServerError;
                response.DevMessage = ex.Message;
                response.UserMessage = "Có lỗi khi lấy danh sách nhân viên";
                return StatusCode(500, response);
            }
        }

        [HttpGet("{code}/employee-count")]
        public async Task<ActionResult<ServiceResponse>> GetEmployeeCountByDepartmentCode(string code)
        {
            var response = new ServiceResponse();
            try
            {
                var department = await _departmentSerice.GetDepartmentByCodeAsync(code);
                if (department == null)
                {
                    response.IsSuccess = false;
                    response.Code = (int)ResponseCode.NotFound;
                    response.UserMessage = "Không tìm thấy phòng ban";
                    return NotFound(response);
                }

                var employees = await _employeeService.GetEmployeesByDepartmentIdAsync(department.DepartmentID);
                var count = 0;
                if (employees != null)
                    count = employees.Count();

                response.Data = count;
                response.IsSuccess = true;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = (int)ResponseCode.InternalServerError;
                response.DevMessage = ex.Message;
                response.UserMessage = "Có lỗi khi đếm nhân viên";
                return StatusCode(500, response);
            }
        }
    }
}
