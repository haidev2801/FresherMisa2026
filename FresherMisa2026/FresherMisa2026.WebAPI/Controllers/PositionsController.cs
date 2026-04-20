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
            try
            {
                var response = new ServiceResponse();
                response.Data = await _positionService.GetPositionByCodeAsync(code);
                response.IsSuccess = true;

                return response;
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ServiceResponse
                {
                    IsSuccess = false,
                    Code = (int)ResponseCode.NotFound,
                    DevMessage = ex.Message
                });
            }
        }
    }
}