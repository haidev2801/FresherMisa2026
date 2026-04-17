using FresherMisa2026.Application.Dtos.Position;
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
            var response = new ServiceResponse();
            response.Data = await _positionService.GetPositionByCodeAsync(code);
            response.IsSuccess = true;

            return response;
        }

        [HttpPost("create-dto")]
        public async Task<IActionResult> CreateDto(CreatePositionDto dto)
        {
            try
            {
                var response = await _positionService.CreatePositionDtoAsync(dto);

                if (!response.IsSuccess)
                    return BadRequest(response);

                return StatusCode((int)ResponseCode.Created, response);
            }

            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}/update-dto")]
        public async Task<IActionResult> UpdateDto(Guid id, UpdatePositionDto dto)
        {
            try
            {
                var response = await _positionService.UpdatePositionDtoAsync(id, dto);

                if (!response.IsSuccess)
                    return BadRequest(response);

                return StatusCode((int)ResponseCode.Success, response);
            }

            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
