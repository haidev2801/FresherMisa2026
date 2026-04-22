using FresherMisa2026.Application.Interfaces.Services;
using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Enums;
using Microsoft.AspNetCore.Mvc;

namespace FresherMisa2026.WebAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    /// <summary>
    /// Controller cơ sở dùng chung cho các thực thể
    /// </summary>
    /// <typeparam name="TEntity">Kiểu thực thể</typeparam>
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
        /// <param name="search">Từ khóa tìm kiếm</param>
        /// <param name="sort">Điều kiện sắp xếp</param>
        /// <param name="pageSize">Số bản ghi mỗi trang</param>
        /// <param name="pageIndex">Trang hiện tại</param>
        /// <param name="searchFields">Danh sách trường tìm kiếm, cách nhau bằng dấu ;</param>
        /// <returns>Kết quả phân trang</returns>
        [HttpGet("Paging")]
        public async Task<ActionResult<ServiceResponse>> GetFilterPaging(
            [FromQuery] string? search,
            [FromQuery] string? sort,
            [FromQuery] int pageSize = 10,
            [FromQuery] int pageIndex = 1,
            [FromQuery] string? searchFields = null
        )
        {
            var pagingRequest = new PagingRequest
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                Search = search ?? string.Empty,
                Sort = sort ?? string.Empty,
                SearchFields = searchFields.Split(";").ToList()
            };
            
            var response = await _baseService.GetPaging(pagingRequest);
            return Ok(response);
        }

        /// <summary>
        /// Danh sách
        /// </summary>
        /// <returns>Danh sách thực thể</returns>
        [HttpGet()]
        public async Task<ActionResult<ServiceResponse>> Get()
        {
            var response = await _baseService.GetEntitiesAsync();
            return Ok(response);
        }

        /// <summary>
        /// Một phần tử
        /// </summary>
        /// <param name="ID">Id thực thể</param>
        /// <returns>Thông tin thực thể</returns>
        [HttpGet("{ID}")]
        public async Task<ActionResult<ServiceResponse>> GetByID(Guid ID)
        {
            var response = await _baseService.GetEntityByIDAsync(ID);

            if (!response.IsSuccess && response.Code == (int)ResponseCode.NotFound)
                return StatusCode(response.Code, response);
            
            return Ok(response);
        }

        /// <summary>
        /// Xóa một phần tử
        /// </summary>
        /// <param name="ID">Id thực thể</param>
        /// <returns>Kết quả xóa</returns>
        [HttpDelete("{ID}")]
        public async Task<ActionResult<ServiceResponse>> DeleteByID(Guid ID)
        {
            var response = await _baseService.DeleteByIDAsync(ID);
            
            if (!response.IsSuccess && response.Code == (int)ResponseCode.NotFound)
                return NotFound(response);
            
            if (!response.IsSuccess && response.Code == (int)ResponseCode.BadRequest)
                return BadRequest(response);
            response.UserMessage = "Xóa thành công";
                
            return Ok(response);
        }

        /// <summary>
        /// Thêm một thực thể mới
        /// </summary>
        /// <param name="entity">Dữ liệu thực thể</param>
        /// <returns>Kết quả thêm mới</returns>
        [HttpPost]
        public async Task<ActionResult<ServiceResponse>> Post([FromBody] TEntity entity)
        {
            try
            {
                var response = await _baseService.InsertAsync(entity);
                
                if (!response.IsSuccess)
                    return BadRequest(response);
                response.UserMessage = "Thêm thành công";

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
        /// <param name="id">Id thực thể</param>
        /// <param name="entity">Dữ liệu cập nhật</param>
        /// <returns>Kết quả cập nhật</returns>
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

            response.UserMessage = "Cập nhật thành công";

            return Ok(response);
        }
    }
}