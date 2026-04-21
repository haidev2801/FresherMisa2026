using FresherMisa2026.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FresherMisa2026.Application.Interfaces
{
    /// <summary>
    /// Interface repository dùng chung cho các thực thể
    /// </summary>
    /// <typeparam name="TEntity">Kiểu thực thể</typeparam>
    public interface IBaseRepository<TEntity>
    {
        /// <summary>
        /// Lấy danh sách thực thể có phân trang
        /// </summary>
        /// <param name="pageSize">Số bản ghi trên mỗi trang</param>
        /// <param name="pageIndex">Chỉ số trang</param>
        /// <param name="search">Từ khóa tìm kiếm</param>
        /// <param name="searchFields">Danh sách trường tìm kiếm</param>
        /// <param name="sort">Điều kiện sắp xếp</param>
        /// <returns>Dữ liệu trang hiện tại và tổng số bản ghi</returns>
        public Task<(IEnumerable<TEntity> Data, int Total)> GetPaging(int pageSize,int pageIndex, string search, List<string> searchFields, string sort);

        /// <summary>
        ///  Lấy danh sách thực thể
        /// </summary>
        /// <returns>Danh sách thực thể</returns>
        /// CREATED BY: NHLONG (07/07/2026)
        Task<IEnumerable<BaseModel>> GetEntitiesAsync();

        /// <summary>
        ///  Lấy bản ghi theo id
        /// </summary>
        /// <param name="entityId">Id của bản ghi</param>
        /// <returns>Bản ghi thông tin 1 bản ghi</returns>
        /// CREATED BY: NHLONG (07/07/2026)
        Task<TEntity> GetEntityByIDAsync(Guid entityId);

        /// <summary>
        /// Xóa bản ghi
        /// </summary>
        /// <param name="entityId">Id của bản ghi</param>
        /// <returns>Số bản ghi bị xóa</returns>
        /// CREATED BY: NHLONG (07/07/2026)
        Task<int> DeleteAsync(Guid entityId);


        /// <summary>
        /// Thêm bản ghi
        /// </summary>
        /// <param name="enitity">Thông tin bản ghi</param>
        /// <returns>Số bản ghi</returns>
        /// CREATED BY: NHLONG (07/07/2026)
        Task<int> InsertAsync(TEntity enitity);

        /// <summary>
        /// Cập nhập thông tin bản ghi
        /// </summary>
        /// <param name="entityId">Id bản ghi</param>
        /// <param name="entity">Thông tin bản ghi</param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        /// CREATED BY: NHLONG (07/07/2026)

        Task<int> UpdateAsync(Guid entityId, TEntity entity);

    }
}
