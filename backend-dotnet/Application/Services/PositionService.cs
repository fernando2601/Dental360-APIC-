using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using DentalSpa.Application.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DentalSpa.Application.Services
{
    public class PositionService : IPositionService
    {
        private readonly IPositionRepository _repository;
        public PositionService(IPositionRepository repository)
        {
            _repository = repository;
        }
        public Task<IEnumerable<Position>> GetAllAsync() => _repository.GetAllAsync();
        public Task<Position?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);
        public Task<Position> CreateAsync(Position position) => _repository.CreateAsync(position);
        public Task<Position?> UpdateAsync(int id, Position position) => _repository.UpdateAsync(id, position);
        public Task<bool> DeleteAsync(int id) => _repository.DeleteAsync(id);
    }
} 