using FresherMisa2026.Application.Interfaces.Services;
using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Enums;
using FresherMisa2026.Entities.Employee;
using FresherMisa2026.Entities.Employee.DTO;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace FresherMisa2026.WebAPI.Controllers
{
    /// <summary>
    /// API quản lý nhân viên.
    /// Các API CRUD cơ bản được kế thừa từ BaseController:
    /// - GET /api/Employees
    /// - GET /api/Employees/{id}
    /// - POST /api/Employees
    /// - PUT /api/Employees/{id}
    /// - DELETE /api/Employees/{id}
    /// </summary>
    [ApiController]
    public class EmployeesController : BaseController<Employee>
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(
            IEmployeeService employeeService) : base(employeeService)
        {
            _employeeService = employeeService;
        }

        /// <summary>
        /// Lấy nhân viên theo mã nhân viên.
        /// </summary>
        /// <param name="code">Mã nhân viên</param>
        /// <returns>Thông tin nhân viên tương ứng</returns>
        [HttpGet("Code/{code}")]
        public async Task<ActionResult<ServiceResponse>> GetByCode(string code)
        {
            var response = new ServiceResponse();
            response.Data = await _employeeService.GetEmployeeByCodeAsync(code);
            response.IsSuccess = true;

            return response;
        }

        /// <summary>
        /// Lấy danh sách nhân viên theo phòng ban.
        /// </summary>
        /// <param name="departmentId">Id phòng ban</param>
        /// <returns>Danh sách nhân viên thuộc phòng ban</returns>
        [HttpGet("Department/{departmentId}")]
        public async Task<ActionResult<ServiceResponse>> GetByDepartmentId(Guid departmentId)
        {
            var response = new ServiceResponse();
            response.Data = await _employeeService.GetEmployeesByDepartmentIdAsync(departmentId);
            response.IsSuccess = true;

            return response;
        }

        /// <summary>
        /// Lấy danh sách nhân viên theo vị trí.
        /// </summary>
        /// <param name="positionId">Id vị trí</param>
        /// <returns>Danh sách nhân viên thuộc vị trí</returns>
        [HttpGet("Position/{positionId}")]
        public async Task<ActionResult<ServiceResponse>> GetByPositionId(Guid positionId)
        {
            var response = new ServiceResponse();
            response.Data = await _employeeService.GetEmployeesByPositionIdAsync(positionId);
            response.IsSuccess = true;

            return response;
        }

        /// <summary>
        /// Lọc danh sách nhân viên theo nhiều điều kiện.
        /// </summary>
        /// <param name="filterRequest">Thông tin điều kiện lọc</param>
        /// <returns>Danh sách nhân viên phù hợp điều kiện</returns>
        [HttpGet("filter")]
        public async Task<ActionResult<ServiceResponse>> Filter([FromQuery] EmployeeFilterRequest filterRequest)
        {
            var response = new ServiceResponse
            {
                Data = await _employeeService.FilterEmployeesAsync(filterRequest),
                IsSuccess = true
            };

            return Ok(response);
        }

        /// <summary>
        /// Mô phỏng race condition khi gửi 2 request tạo mới cùng EmployeeCode.
        /// </summary>
        [HttpPost("test-race-condition")]
        public async Task<ActionResult<ServiceResponse>> TestRaceCondition([FromBody] EmployeeRaceConditionTestRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.EmployeeCode))
            {
                return BadRequest(new ServiceResponse
                {
                    IsSuccess = false,
                    Code = (int)ResponseCode.BadRequest,
                    UserMessage = "EmployeeCode là bắt buộc"
                });
            }

            var employee1 = CreateEmployeeForRaceTest(request, Guid.NewGuid());
            var employee2 = CreateEmployeeForRaceTest(request, Guid.NewGuid());

            var task1 = ExecuteInsertForRaceTestAsync("request-1", employee1);
            var task2 = ExecuteInsertForRaceTestAsync("request-2", employee2);

            var results = await Task.WhenAll(task1, task2);

            var response = new ServiceResponse
            {
                IsSuccess = true,
                Code = (int)ResponseCode.Success,
                Data = new
                {
                    request.EmployeeCode,
                    SuccessCount = results.Count(r => r.IsSuccess),
                    FailedCount = results.Count(r => !r.IsSuccess),
                    Results = results
                }
            };

            return Ok(response);
        }

        private Employee CreateEmployeeForRaceTest(EmployeeRaceConditionTestRequest request, Guid employeeId)
        {
            return new Employee
            {
                EmployeeID = employeeId,
                EmployeeCode = request.EmployeeCode,
                EmployeeName = request.EmployeeName,
                DepartmentID = request.DepartmentID,
                PositionID = request.PositionID,
                Gender = request.Gender,
                DateOfBirth = request.DateOfBirth,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                Address = request.Address,
                Salary = request.Salary,
                CreatedDate = DateTime.Now
            };
        }

        private async Task<RaceInsertResult> ExecuteInsertForRaceTestAsync(string requestName, Employee employee)
        {
            try
            {
                var serviceResponse = await _employeeService.InsertAsync(employee);
                return new RaceInsertResult
                {
                    Request = requestName,
                    IsSuccess = serviceResponse.IsSuccess,
                    Code = serviceResponse.Code,
                    Data = serviceResponse.Data,
                    UserMessage = serviceResponse.UserMessage,
                    DevMessage = serviceResponse.DevMessage
                };
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                return new RaceInsertResult
                {
                    Request = requestName,
                    IsSuccess = false,
                    Code = (int)ResponseCode.BadRequest,
                    UserMessage = "Mã nhân viên đã tồn tại",
                    DevMessage = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new RaceInsertResult
                {
                    Request = requestName,
                    IsSuccess = false,
                    Code = (int)ResponseCode.InternalServerError,
                    UserMessage = "Có lỗi xảy ra vui lòng liên hệ Misa!",
                    DevMessage = ex.Message
                };
            }
        }

        private class RaceInsertResult
        {
            public string Request { get; set; } = string.Empty;
            public bool IsSuccess { get; set; }
            public int Code { get; set; }
            public object? Data { get; set; }
            public object? UserMessage { get; set; }
            public object? DevMessage { get; set; }
        }
    }
}