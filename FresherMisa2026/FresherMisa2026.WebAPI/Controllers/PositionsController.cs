using FresherMisa2026.Application.Interfaces.Services;
using FresherMisa2026.Entities.Position;
using Microsoft.AspNetCore.Mvc;

namespace FresherMisa2026.WebAPI.Controllers
{
    [ApiController]
    public class PositionsController : BaseController<Position>
    {
        public PositionsController(IBaseService<Position> baseService) : base(baseService)
        {
        }
    }
}