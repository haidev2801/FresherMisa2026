using FresherMisa2026.Application.Interfaces;
using FresherMisa2026.Application.Interfaces.Services;
using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Department;
using FresherMisa2026.Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using static Dapper.SqlMapper;

namespace FresherMisa2026.WebAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class BaseController<TEntity> : ControllerBase
    {

        private readonly IBaseService<TEntity> _baseService;

        public BaseController(IBaseService<TEntity> baseService)
        {
            _baseService = baseService;
        }

        /// <summary>
        /// Danh sách paging
        /// </summary>
        /// <returns></returns>
        [HttpGet("Paging")]
        public async Task<ActionResult<ServiceResponse>> GetFilterPaging(
            [FromQuery] string? search,
            [FromQuery] string? sort,
            [FromQuery] int pageSize,
            [FromQuery] int pageIndex,
            [FromQuery] string? searchFields
            )
        {
            var pagingRequest = new PagingRequest
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                Search = search ?? string.Empty,
                Sort = sort ?? string.Empty,
                SearchFields = searchFields ?? string.Empty
            };
            
            var response = await _baseService.GetFilterPagingAsync(pagingRequest);

            return Ok(response);
        }

        /// <summary>
        /// Danh sách
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        public async Task<ActionResult<ServiceResponse>> Get()
        {
            var response = await _baseService.GetEntitiesAsync();

            return Ok(response);
        }

        /// <summary>
        /// Một phần tử
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpGet("{ID}")]
        public async Task<ActionResult<ServiceResponse>> GetByID(Guid ID)
        {
            var response = await _baseService.GetEntityByIDAsync(ID);

            if (!response.IsSuccess && response.Code == (int)ResponseCode.NotFound)
                return NotFound(response);
            
            return Ok(response);
        }

        /// <summary>
        /// Xóa một phần tử
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpDelete("{ID}")]
        public async Task<ActionResult<ServiceResponse>> DeleteByID(Guid ID)
        {
            var response = await _baseService.DeleteByIDAsync(ID);
            
            if (!response.IsSuccess && response.Code == (int)ResponseCode.NotFound)
                return NotFound(response);
            
            if (!response.IsSuccess && response.Code == (int)ResponseCode.BadRequest)
                return BadRequest(response);
                
            return Ok(response);
        }

        /// <summary>
        /// Thêm một thực thể mới
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>Sô bản ghi bị ảnh hưởng</returns>
        /// CreatedBy: DVHAI 07/07/2026
        [HttpPost]
        public async Task<ActionResult<ServiceResponse>> Post([FromBody] TEntity entity)
        {
            try
            {
                var response = await _baseService.InsertAsync(entity);
                
                if (!response.IsSuccess)
                    return BadRequest(response);

                return StatusCode((int)ResponseCode.Created, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Sửa một thực thể
        /// </summary>
        /// <param name="id">id của bản ghi</param>
        /// <param name="entity">thông tin của bản ghi</param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        /// CreatedBy: DVHAI 07/07/2021
        [HttpPut("{id}")]
        public async Task<ActionResult<ServiceResponse>> Put([FromRoute] string id, [FromBody] TEntity entity)
        {
            var response = await _baseService.UpdateAsync(Guid.Parse(id), entity);

            if (!response.IsSuccess)
            {
                if (response.Code == (int)ResponseCode.NotFound)
                    return NotFound(response);
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}