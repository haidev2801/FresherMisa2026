using FresherMisa2026.Entities.Position;
using System;

namespace FresherMisa2026.Application.Interfaces.Repositories
{
    /// <summary>
    /// Interface truy cập dữ liệu vị trí
    /// </summary>
    public interface IPositionRepository : IBaseRepository<Position>
    {
        /// <summary>
        /// Lấy vị trí theo mã
        /// </summary>
        /// <param name="code">Mã vị trí</param>
        /// <returns>Vị trí tìm thấy</returns>
        Task<Position> GetPositionByCode(string code);
    }
}