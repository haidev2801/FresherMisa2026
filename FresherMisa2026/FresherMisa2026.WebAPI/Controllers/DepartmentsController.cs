using FresherMisa2026.Application.Interfaces;
using FresherMisa2026.Application.Interfaces.Services;
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
        public DepartmentsController(IBaseService<Department> baseService) : base(baseService)
        {
        }
    }
}
