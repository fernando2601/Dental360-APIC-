using System.Collections.Generic;
using System.Threading.Tasks;
using DentalSpa.Domain.Entities;

namespace DentalSpa.Domain.Interfaces
{
    public interface IPositionRepository
    {
        Task<IEnumerable<Position>> GetAllAsync();
        Task<Position?> GetByIdAsync(int id);
        Task<Position> CreateAsync(Position position);
        Task<Position?> UpdateAsync(int id, Position position);
        Task<bool> DeleteAsync(int id);
    }
} 