using FresherMisa2026.Application.Dtos.Position;
using FresherMisa2026.Application.Interfaces;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Application.Interfaces.Services;
using FresherMisa2026.Application.Mappers.Positions;
using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Position;

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

        public async Task<ServiceResponse> CreatePositionDtoAsync(CreatePositionDto dto)
        {
            var newPosition = dto.ToPositionFromCreateDto();

            return await InsertAsync(newPosition);
        }

        public async Task<Position> GetPositionByCodeAsync(string code)
        {
            var position = await _positionRepository.GetPositionByCode(code);
            if (position == null)
                throw new Exception("Position not found");

            return position;
        }

        public async Task<ServiceResponse> UpdatePositionDtoAsync(Guid id, UpdatePositionDto dto)
        {
            var position = await _positionRepository.GetEntityByIDAsync(id);

            if (position == null)
                throw new Exception("Position not found");

            return await UpdateAsync(id, position);
        }

        protected override List<ValidationError> ValidateCustom(Position position)
        {
            var errors = new List<ValidationError>();

            if (!string.IsNullOrEmpty(position.PositionCode) && position.PositionCode.Length > 20)
            {
                errors.Add(new ValidationError("PositionCode", "Mã vị trí không được vượt quá 20 ký tự"));
            }

            return errors;
        }
    }
}
