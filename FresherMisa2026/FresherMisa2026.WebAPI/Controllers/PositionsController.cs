using FresherMisa2026.Application.Interfaces.Services;
using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Enums;
using FresherMisa2026.Entities.Position;
using Microsoft.AspNetCore.Mvc;

namespace FresherMisa2026.WebAPI.Controllers
{
    [ApiController]
    public class PositionsController : BaseController<Position>
    {
        private readonly IPositionService _positionService;

        public PositionsController(
            IPositionService positionService) : base(positionService)
        {
            _positionService = positionService;
        }

        [HttpGet("Code/{code}")]
        public async Task<ActionResult<ServiceResponse>> GetByCode(string code)
        {
            var position = await _positionService.GetPositionByCodeAsync(code);
            if (position == null)
            {
                return NotFound(new ServiceResponse
                {
                    IsSuccess = false,
                    Code = (int)ResponseCode.NotFound,
                    UserMessage = "Không tìm thấy vị trí",
                    DevMessage = $"Không tìm thấy vị trí có mã '{code}'"
                });
            }

            return Ok(new ServiceResponse
            {
                IsSuccess = true,
                Code = (int)ResponseCode.Success,
                Data = position
            });
        }
    }
}
