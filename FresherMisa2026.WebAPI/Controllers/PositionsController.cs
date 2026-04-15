using FresherMisa2026.Application.Interfaces.Services;
using FresherMisa2026.Entities;
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

        /// <summary>
        /// Lấy position theo code
        /// </summary>
        /// <returns></returns>
        /// Created By: dvhai (15/04/2026)
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