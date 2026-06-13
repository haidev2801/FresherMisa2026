using FresherMisa2026.Application.Interfaces;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Application.Interfaces.Services;
using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Position;
using System;
using System.Collections.Generic;

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

        public async Task<Position> GetPositionByCodeAsync(string code)
        {
            var position = await _positionRepository.GetPositionByCode(code);
            if (position == null)
                throw new Exception("Position not found");

            return position;
        }

        /// <summary>
        /// Created by: ChucTC1 - 18/4/2026
        /// Modified by: ChucTC1 - 21/4/2026
        /// Sửa: vì chưa check null ở đây nên thêm điều kiện check null để tránh lỗi khi position.PositionCode là null
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        protected override List<ValidationError> ValidateCustom(Position position)
        {
            var errors = new List<ValidationError>();

            if (!string.IsNullOrEmpty(position.PositionCode) && position.PositionCode.Length > 20)
            {
                errors.Add(new ValidationError("PositionCode", "Mã vị trí không được vượt quá 20 ký tự"));
            }
            
            if (!string.IsNullOrEmpty(position.PositionCode))
            {
                var existing = _positionRepository.GetPositionByCode(position.PositionCode).Result;
                if (existing != null && existing.PositionID != position.PositionID)
                {
                    errors.Add(new ValidationError("PositionCode", "Mã vị trí đã tồn tại"));
                }
            }
            return errors;
        }
    }
}