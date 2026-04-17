using FresherMisa2026.Entities.Position;
using System;

namespace FresherMisa2026.Application.Interfaces.Repositories
{
    public interface IPositionRepository : IBaseRepository<Position>
    {
        /// <summary>
        /// Lấy position theo code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        /// Created by: Phuong (17/04/2026)
        Task<Position> GetPositionByCode(string code);
    }
}