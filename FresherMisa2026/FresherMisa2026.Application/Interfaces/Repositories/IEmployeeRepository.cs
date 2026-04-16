using FresherMisa2026.Application.Dtos.Employee;
using FresherMisa2026.Entities.Employee;

namespace FresherMisa2026.Application.Interfaces.Repositories
{
    public interface IEmployeeRepository : IBaseRepository<Employee>
    {
        Task<Employee> GetEmployeeByCode(string code);
        Task<IEnumerable<Employee>> GetEmployeesByDepartmentId(Guid departmentId);
        Task<IEnumerable<Employee>> GetEmployeesByPositionId(Guid positionId);
        Task<(long Total, IEnumerable<EmployeeDto> Data)>
            GetEmployeesFilter(GetEmployeeFilterDto dto);
    }
}
