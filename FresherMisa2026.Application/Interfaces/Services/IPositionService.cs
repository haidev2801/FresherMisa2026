using FresherMisa2026.Entities.Position;
using System;
using System.Collections.Generic;
using System.Text;

namespace FresherMisa2026.Application.Interfaces.Services
{
    public interface IPositionService : IBaseService<Position>
    {
        /// <summary>
        /// Lấy position theo code
        /// </summary>
        /// <param name="code">Mã position</param>
        /// <returns>Position tìm thấy</returns>
        Task<Position> GetPositionByCodeAsync(string code);
    }
}