using FresherMisa2026.Entities;
using System;
using System.Collections.Generic;

namespace FresherMisa2026.Application.Interfaces.Services
{
    /// <summary>
    /// Interface service chung cho các entity, định nghĩa các phương thức service cơ bản.
    /// </summary>
    public interface IBaseService<TEntity>
    {
        /// <summary>
        /// Lấy tất cả bản ghi
        /// </summary>
        /// <returns>Danh sách bản ghi</returns>
        /// CREATED BY: DVHAI 11/07/2026
        Task<ServiceResponse> GetEntitiesAsync();

        /// <summary>
        /// Lấy bản ghi theo id
        /// </summary>
        /// <param name="entityId">Id của bản ghi</param>
        /// <returns>Bản ghi thông tin 1 bản ghi</returns>
        /// CREATED BY: DVHAI (07/07/2026)
        Task<ServiceResponse> GetEntityByIDAsync(Guid entityId);

        /// <summary>
        /// Xóa bản ghi theo id, trả về true nếu xóa thành công
        /// </summary>
        /// <param name="entityId">Id bản ghi</param>
        /// <returns>ServiceResponse</returns>
        /// CREATED BY: DVHAI (07/07/2026)
        Task<ServiceResponse> DeleteByIDAsync(Guid entityId);

        /// <summary>
        /// Thêm bản ghi và trả về ServiceResponse
        /// </summary>
        /// <param name="entity">Thực thể cần thêm</param>
        /// <returns>ServiceResponse</returns>
        /// CREATED BY: DVHAI (11/07/2026)
        Task<ServiceResponse> InsertAsync(TEntity entity);

        /// <summary>
        /// Cập nhật bản ghi và trả về ServiceResponse
        /// </summary>
        /// <param name="entityId">Id bản ghi</param>
        /// <param name="entity">Thông tin bản ghi</param>
        /// <returns>ServiceResponse</returns>
        /// CREATED BY: DVHAI (11/07/2021)
        Task<ServiceResponse> UpdateAsync(Guid entityId, TEntity entity);

        /// <summary>
        /// Lấy danh sách thực thể paging
        /// </summary>
        /// <param name="pagingRequest">Thông tin phân trang</param>
        /// <returns>Danh sách thực thể phân trang</returns>
        /// CREATED BY: DVHAI (07/07/2026)
        Task<ServiceResponse> GetFilterPagingAsync(PagingRequest pagingRequest);
    }
}