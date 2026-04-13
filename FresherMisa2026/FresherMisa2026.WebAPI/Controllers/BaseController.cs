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
    /// <summary>
    /// Controller generic dùng chung cho các entity. Cung cấp các endpoint chuẩn: GET list, GET by id, POST, PUT, DELETE.
    /// Các controller cụ thể sẽ kế thừa BaseController<TEntity> và được inject IBaseService<TEntity>.
    /// </summary>
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
        [HttpGet("paging")]
        public async Task<ActionResult<ServiceResponse>> GetFilterPaping(
            [FromQuery] string search,
            [FromQuery] string sort,
            [FromQuery] int pageSize,
            [FromQuery] int pageIndex
            )
        {
            var response = new ServiceResponse();
            response.Data = await _baseService.GetEntities();
            response.IsSuccess = true;

            return response;
        }

        /// <summary>
        /// Danh sách
        /// GET /api/{controller}
        /// Trả về ServiceResponse với Data là danh sách entity
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        public async Task<ActionResult<ServiceResponse>> Get()
        {
            var response = new ServiceResponse();
            response.Data = await _baseService.GetEntities();
            response.IsSuccess = true;

            return response;
        }

        /// <summary>
        /// Một phần tử
        /// GET /api/{controller}/{ID}
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpGet("{ID}")]
        public async Task<ActionResult<ServiceResponse>> GetByID(Guid ID)
        {
            var response = new ServiceResponse();
            response.Data = await _baseService.GetEntityByID(ID);
            response.IsSuccess = true;

            if (response.Data == null)
                return NotFound();
            
            return response;
        }

        /// <summary>
        /// Xóa một phần tử
        /// DELETE /api/{controller}/{ID}
        /// Trả về ServiceResponse với IsSuccess=true nếu xóa thành công
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpDelete("{ID}")]
        public async Task<ActionResult<ServiceResponse>> DeleteByID(Guid ID)
        {
            var response = new ServiceResponse();
            bool success = await _baseService.DeleteByID(ID);
            response.IsSuccess = success;
            response.Data = ID;
            return response;
        }

        /// <summary>
        /// Thêm một thực thể mới
        /// POST /api/{controller}
        /// Trả về status code 201 Created nếu thành công, 400 nếu validate lỗi.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>Sô bản ghi bị ảnh hưởng</returns>
        /// CreatedBy: DVHAI 07/07/2026
        [HttpPost]
        public async Task<ActionResult<ServiceResponse>> Post([FromBody] TEntity entity)
        {
            var serviceResult = new ServiceResponse();
            try
            {
                serviceResult = await _baseService.Insert(entity);
                if (!serviceResult.IsSuccess)
                    return BadRequest(serviceResult);

                return StatusCode((int)ResponseCode.Created, serviceResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Sửa một thực thể
        /// PUT /api/{controller}/{id}
        /// Trả về 200 OK nếu thành công, 400/404/500 tuỳ trường hợp lỗi.
        /// </summary>
        /// <param name="id">id của bản ghi</param>
        /// <param name="entity">thông tin của bản ghi</param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        /// CreatedBy: DVHAI 07/07/2021
        [HttpPut("{id}")]
        public async Task<ActionResult<ServiceResponse>> Put([FromRoute] string id, [FromBody] TEntity entity)
        {
            var serviceResult = await _baseService.Update(Guid.Parse(id), entity);

            if (!serviceResult.IsSuccess)
                return StatusCode(StatusCodes.Status400BadRequest, serviceResult);
            else if (serviceResult.Code == (int)ResponseCode.InternalServerError)
                return StatusCode(StatusCodes.Status500InternalServerError, serviceResult);
            else if (serviceResult.Code == (int)ResponseCode.NotFound)
                return StatusCode(StatusCodes.Status404NotFound, serviceResult);

            return Ok(serviceResult);
        }
    }
}
