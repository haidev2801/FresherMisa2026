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

        protected override List<ValidationError> ValidateCustom(Position position)
        {
            var errors = new List<ValidationError>();

            var existedPosition = string.IsNullOrWhiteSpace(position.PositionCode)
                ? null
                : _positionRepository.GetPositionByCode(position.PositionCode).Result;

            if (existedPosition != null
                && (position.State == ModelSate.Add || existedPosition.PositionID != position.PositionID))
            {
                errors.Add(new ValidationError("PositionCode", "Mã vị trí đã tồn tại"));
            }

            if (string.IsNullOrWhiteSpace(position.PositionCode))
            {
                errors.Add(new ValidationError("PositionCode", "Mã vị trí không được để trống"));
            }

            if (string.IsNullOrWhiteSpace(position.PositionName))
            {
                errors.Add(new ValidationError("PositionName", "Tên vị trí không được để trống"));
            }

            if (!string.IsNullOrEmpty(position.PositionCode) && position.PositionCode.Length > 20)
            {
                errors.Add(new ValidationError("PositionCode", "Mã vị trí không được vượt quá 20 ký tự"));
            }

            return errors;
        }
    }
}