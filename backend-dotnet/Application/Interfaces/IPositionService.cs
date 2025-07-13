using System.Collections.Generic;
using System.Threading.Tasks;
using DentalSpa.Domain.Entities;

namespace DentalSpa.Application.Interfaces
{
    public interface IPositionService
    {
        Task<IEnumerable<Position>> GetAllAsync();
        Task<Position?> GetByIdAsync(int id);
        Task<Position> CreateAsync(Position position);
        Task<Position?> UpdateAsync(int id, Position position);
        Task<bool> DeleteAsync(int id);
    }
} 