using FresherMisa2026.Application.Dtos.Employee;
using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Employee;

namespace FresherMisa2026.Application.Interfaces.Services
{
    public interface IEmployeeService : IBaseService<Employee>
    {
        Task<Employee> GetEmployeeByCodeAsync(string code);
        Task<IEnumerable<Employee>> GetEmployeesByDepartmentIdAsync(Guid departmentId);
        Task<IEnumerable<Employee>> GetEmployeesByPositionIdAsync(Guid positionId);
        Task<ServiceResponse> GetEmployeeFilterAsync(GetEmployeeFilterDto dto);
        Task<ServiceResponse> UpdateDtoAsync(Guid id, UpdateEmployeeDto dto);
        Task<ServiceResponse> CreateDtoAsync(CreateEmployeeDto dto);
    }
}