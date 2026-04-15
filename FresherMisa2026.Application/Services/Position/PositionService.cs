using FresherMisa2026.Application.Interfaces;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Application.Interfaces.Services;
using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Position;
using System;
using System.Collections.Generic;
using System.Text;

namespace FresherMisa2026.Application.Services
{
    public class PositionService : BaseService<Position>, IPositionService
    {
        private readonly IPositionRepository _positionRepository;

        public PositionService(
            IBaseRepository<Position> baseRepository,
            IPositionRepository positionRepository
            ) : base(baseRepository)
        {
            _positionRepository = positionRepository;
        }

        /// <summary>
        /// Lấy position theo code
        /// </summary>
        /// <param name="code">Mã position</param>
        /// <returns>Position tìm thấy</returns>
        /// Created By: dvhai (15/04/2026)
        public async Task<Position> GetPositionByCodeAsync(string code)
        {
            var position = await _positionRepository.GetPositionByCode(code);
            if (position == null)
                throw new Exception("Position not found");

            return position;
        }

        #region OVERRIDE METHODS

        /// <summary>
        /// Validate tùy chỉnh cho Position
        /// </summary>
        protected override List<ValidationError> ValidateCustom(Position position)
        {
            var errors = new List<ValidationError>();

            // Ví dụ: Kiểm tra mã vị trí không được vượt quá 20 ký tự
            if (!string.IsNullOrEmpty(position.PositionCode) && position.PositionCode.Length > 20)
            {
                errors.Add(new ValidationError("PositionCode", "Mã vị trí không được vượt quá 20 ký tự"));
            }

            return errors;
        }

        #endregion OVERRIDE METHODS
    }
}