using FresherMisa2026.Application.Constants.Regex;
using FresherMisa2026.Application.Dtos.Employee;
using FresherMisa2026.Application.Interfaces;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Application.Interfaces.Services;
using FresherMisa2026.Application.Mappers.Employees;
using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Employee;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace FresherMisa2026.Application.Services
{
    public class EmployeeService : BaseService<Employee>, IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IPositionRepository _positionRepository;
        private readonly ICacheService _cacheService;

        public EmployeeService(
            IBaseRepository<Employee> baseRepository,
            IEmployeeRepository employeeRepository,
            ICacheService cacheService,
            IDepartmentRepository departmentRepository,
            IPositionRepository positionRepository) : base(baseRepository, cacheService)
        {
            _employeeRepository = employeeRepository;
            _cacheService = cacheService;
            _departmentRepository = departmentRepository;
            _positionRepository = positionRepository;
        }

        public async Task<Employee> GetEmployeeByCodeAsync(string code)
        {
            var employee = await _employeeRepository.GetEmployeeByCode(code);
            if (employee == null)
                throw new Exception("Employee not found");

            return employee;
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentIdAsync(Guid departmentId)
        {
            return await _employeeRepository.GetEmployeesByDepartmentId(departmentId);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByPositionIdAsync(Guid positionId)
        {
            return await _employeeRepository.GetEmployeesByPositionId(positionId);
        }

        public async Task<ServiceResponse> GetEmployeeFilterAsync(GetEmployeeFilterDto dto)
        {
            var (total, data) = await _employeeRepository.GetEmployeesFilter(dto);

            var response = new PagingResponse<EmployeeDto>
            {
                Total = total,
                Data = data.ToList()
            };

            return CreateSuccessResponse(response);
        }

        protected override List<ValidationError> ValidateCustom(Employee employee)
        {
            var errors = new List<ValidationError>();

            var existing = _employeeRepository.GetEmployeeByCode(employee.EmployeeCode).Result;
            var department = _departmentRepository.GetEntityByIDAsync(employee.DepartmentID).Result;
            var position = _positionRepository.GetEntityByIDAsync(employee.PositionID).Result;

            if (department == null)
            {
                errors.Add(new ValidationError("DepartmentId.Invalid",
                    "Department không tồn tại"));
            }

            if (position == null)
            {
                errors.Add(new ValidationError("PositionId.Invalid",
                    "Position không tồn tại"));
            }

            if (existing != null && employee.EmployeeID != existing.EmployeeID)
            {
                errors.Add(new ValidationError("EmployeeCode.Existed",
                    "Mã nhân viên không được trùng lặp"));
            }

            if (!string.IsNullOrWhiteSpace(employee.Email))
            {
                try
                {
                    _ = new MailAddress(employee.Email);
                }
                catch (FormatException)
                {
                    errors.Add(new ValidationError("Email.Invalid",
                        "Email không đúng định dạng"));
                }
            }

            if (!string.IsNullOrWhiteSpace(employee.PhoneNumber) &&
                !Regex.IsMatch(employee.PhoneNumber, EmployeeRegex.PHONE_REGEX))
            {
                errors.Add(new ValidationError("PhoneNumber.Invalid",
                    "Số điện thoại không đúng định dạng"));
            }

            if (employee.DateOfBirth.HasValue && employee.DateOfBirth.Value.Date >= DateTime.Today)
            {
                errors.Add(new ValidationError("DateOfBirth.Invalid",
                    "Ngày sinh phải nhỏ hơn ngày hiện tại"));
            }

            if (!string.IsNullOrEmpty(employee.EmployeeCode) && employee.EmployeeCode.Length > 20)
            {
                errors.Add(new ValidationError("EmployeeCode", "Mã nhân viên không được vượt quá 20 ký tự"));
            }

            if (string.IsNullOrEmpty(employee.EmployeeName))
            {
                errors.Add(new ValidationError("EmployeeName", "Tên nhân viên không được để trống"));
            }

            return errors;
        }

        public async Task<ServiceResponse> UpdateDtoAsync(Guid id, UpdateEmployeeDto dto)
        {
            var employee = await _employeeRepository.GetEntityByIDAsync(id);

            if (employee == null)
                throw new Exception("Employee not found");

            return await UpdateAsync(id, employee);
        }

        public async Task<ServiceResponse> CreateDtoAsync(CreateEmployeeDto dto)
        {
            var newEmployee = dto.ToEmployeeFromCreateDto();

            return await InsertAsync(newEmployee);
        }
    }
}