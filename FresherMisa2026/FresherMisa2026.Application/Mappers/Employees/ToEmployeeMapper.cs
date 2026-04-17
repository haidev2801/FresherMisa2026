using FresherMisa2026.Application.Dtos.Employee;
using FresherMisa2026.Entities.Employee;

namespace FresherMisa2026.Application.Mappers.Employees
{
    public static class ToEmployeeMapper
    {
        public static Employee ToEmployeeFromCreateDto(this CreateEmployeeDto dto)
        {
            return new Employee
            {
                EmployeeID = dto.EmployeeID,
                EmployeeCode = dto.EmployeeCode,
                EmployeeName = dto.EmployeeName,
                Gender = dto.Gender,
                DateOfBirth = dto.DateOfBirth,
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email,
                Address = dto.Address,
                DepartmentID = dto.DepartmentID,
                PositionID = dto.PositionID,
                Salary = dto.Salary,
            };
        }
    }
}
