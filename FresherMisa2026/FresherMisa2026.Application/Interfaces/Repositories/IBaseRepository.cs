using FresherMisa2026.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FresherMisa2026.Application.Interfaces
{
    public interface IBaseRepository<TEntity>
    {
        public Task<(IEnumerable<TEntity> Data, int Total)> GetPaging(int pageSize,int pageIndex, string search, List<string> searchFields, string sort);

        // <summary>
        ///  Lấy danh sách thực thể
        /// </summary>
        /// <param name="entityId">Id của bản ghi</param>
        /// <returns>Bản ghi thông tin 1 bản ghi</return
        /// CREATED BY: NHLONG (07/07/2026)
        Task<IEnumerable<BaseModel>> GetEntitiesAsync();

        // <summary>
        ///  Lấy bản ghi theo id
        /// </summary>
        /// <param name="entityId">Id của bản ghi</param>
        /// <returns>Bản ghi thông tin 1 bản ghi</return
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
