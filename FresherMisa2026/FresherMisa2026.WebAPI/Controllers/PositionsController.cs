using FresherMisa2026.Application.Interfaces.Services;
using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Position;
using Microsoft.AspNetCore.Mvc;

namespace FresherMisa2026.WebAPI.Controllers
{
    [ApiController]
    /// <summary>
    /// API quản lý vị trí
    /// </summary>
    public class PositionsController : BaseController<Position>
    {
        private readonly IPositionService _positionService;

        public PositionsController(
            IPositionService positionService) : base(positionService)
        {
            _positionService = positionService;
        }

        /// <summary>
        /// Lấy vị trí theo mã
        /// </summary>
        /// <param name="code">Mã vị trí</param>
        /// <returns>Thông tin vị trí</returns>
        [HttpGet("Code/{code}")]
        public async Task<ActionResult<ServiceResponse>> GetByCode(string code)
        {
            var response = new ServiceResponse();
            response.Data = await _positionService.GetPositionByCodeAsync(code);
            response.IsSuccess = true;

            return response;
        }
    }
}