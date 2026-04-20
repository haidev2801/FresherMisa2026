using FresherMisa2026.Application.Interfaces.Services;
using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Enums;
using FresherMisa2026.Entities.Employee;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

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
            var employee = await _employeeService.GetEmployeeByCodeAsync(code);
            if (employee == null)
            {
                return NotFound(new ServiceResponse
                {
                    IsSuccess = false,
                    Code = (int)ResponseCode.NotFound,
                    UserMessage = "Không tìm thấy nhân viên",
                    DevMessage = $"Không tìm thấy nhân viên có mã '{code}'"
                });
            }

            return Ok(new ServiceResponse
            {
                IsSuccess = true,
                Code = (int)ResponseCode.Success,
                Data = employee
            });
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
        public async Task<ActionResult<ServiceResponse>> GetEmployeesFilter(
            [FromQuery] Guid? departmentId, 
            [FromQuery] Guid? positionId, 
            [FromQuery] string? salaryFrom, 
            [FromQuery] string? salaryTo,
            [FromQuery] int? gender,
            [FromQuery] DateTime? hireDateFrom,
            [FromQuery] DateTime? hireDateTo,
            [FromQuery] int pageSize = 10,
            [FromQuery] int pageIndex = 1
        )
        {
            var validateResponse = ValidateEmployeeFilter(
                salaryFrom,
                salaryTo,
                gender,
                hireDateFrom,
                hireDateTo,
                pageSize,
                pageIndex);

            if (validateResponse != null)
            {
                return BadRequest(validateResponse);
            }

            var response = new ServiceResponse();
            response.Data = await _employeeService.GetEmployeesFilterAsync(
                departmentId,
                positionId,
                salaryFrom,
                salaryTo,
                gender,
                hireDateFrom,
                hireDateTo,
                pageSize,
                pageIndex);
            response.IsSuccess = true;
            response.Code = (int)ResponseCode.Success;

            return Ok(response);
        }

        private static ServiceResponse? ValidateEmployeeFilter(
            string? salaryFrom,
            string? salaryTo,
            int? gender,
            DateTime? hireDateFrom,
            DateTime? hireDateTo,
            int pageSize,
            int pageIndex)
        {
            if (gender.HasValue && gender.Value != 0 && gender.Value != 1 && gender.Value != 2)
            {
                return CreateBadRequestResponse("Giới tính không hợp lệ");
            }

            var salaryFromValid = TryParseSalary(salaryFrom, out var salaryFromValue);
            var salaryToValid = TryParseSalary(salaryTo, out var salaryToValue);

            if (!salaryFromValid || !salaryToValid)
            {
                return CreateBadRequestResponse("Khoảng lương không hợp lệ");
            }

            if (salaryFromValue.HasValue && salaryToValue.HasValue && salaryFromValue > salaryToValue)
            {
                return CreateBadRequestResponse("Lương từ phải nhỏ hơn hoặc bằng lương đến");
            }

            if (hireDateFrom.HasValue && hireDateTo.HasValue && hireDateFrom.Value.Date > hireDateTo.Value.Date)
            {
                return CreateBadRequestResponse("Ngày vào làm từ phải nhỏ hơn hoặc bằng ngày vào làm đến");
            }

            if (pageSize < 1)
            {
                return CreateBadRequestResponse("Kích thước trang phải lớn hơn 0");
            }

            if (pageIndex < 1)
            {
                return CreateBadRequestResponse("Số trang phải lớn hơn 0");
            }

            return null;
        }

        private static bool TryParseSalary(string? value, out decimal? salary)
        {
            salary = null;

            if (string.IsNullOrWhiteSpace(value))
            {
                return true;
            }

            if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var parsedValue))
            {
                salary = parsedValue;
                return true;
            }

            return false;
        }

        private static ServiceResponse CreateBadRequestResponse(string message)
        {
            return new ServiceResponse
            {
                IsSuccess = false,
                Code = (int)ResponseCode.BadRequest,
                UserMessage = message,
                DevMessage = message
            };
        }
    }
}
