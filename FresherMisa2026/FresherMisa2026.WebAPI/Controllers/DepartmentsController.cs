using FresherMisa2026.Application.Interfaces;
using FresherMisa2026.Application.Interfaces.Services;
using FresherMisa2026.Application.Services;
using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Department;
using Microsoft.AspNetCore.Mvc;

namespace FresherMisa2026.WebAPI.Controllers
{
    /// <summary>
    /// Controller cho entity Department. Kế thừa BaseController để tái sử dụng các endpoint CRUD chung.
    /// Nếu cần logic đặc thù cho Department, có thể override hoặc thêm action mới tại đây.
    /// </summary>
    [ApiController]
    public class DepartmentsController : BaseController<Department>
    {
        private readonly IDepartmentSerice _departmentSerice;

        public DepartmentsController(IDepartmentSerice departmentSerice) : base(departmentSerice)
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
    }
}
