using FresherMisa2026.Entities.Position;
using System;
using System.Collections.Generic;
using System.Text;

namespace FresherMisa2026.Application.Interfaces.Repositories
{
    public interface IPositionRepository : IBaseRepository<Position>
    {
        /// <summary>
        /// Lấy position theo code
        /// </summary>
        /// <param name="code">Mã position</param>
        /// <returns>Position tìm thấy hoặc null</returns>
        Task<Position> GetPositionByCode(string code);
    }
}