using FresherMisa2026.Application.Dtos.Employee;
using FresherMisa2026.Application.Interfaces;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Application.Interfaces.Services;
using FresherMisa2026.Application.Mappers.Employees;
using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Employee;

namespace FresherMisa2026.Application.Services
{
    public class EmployeeService : BaseService<Employee>, IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ICacheService _cacheService;

        public EmployeeService(
            IBaseRepository<Employee> baseRepository,
            IEmployeeRepository employeeRepository,
            ICacheService cacheService) : base(baseRepository, cacheService)
        {
            _employeeRepository = employeeRepository;
            _cacheService = cacheService;
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