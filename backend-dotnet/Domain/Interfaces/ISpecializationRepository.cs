using System.Collections.Generic;
using System.Threading.Tasks;
using DentalSpa.Domain.Entities;

namespace DentalSpa.Domain.Interfaces
{
    public interface ISpecializationRepository
    {
        Task<IEnumerable<Specialization>> GetAllAsync();
        Task<Specialization?> GetByIdAsync(int id);
        Task<Specialization> CreateAsync(Specialization specialization);
        Task<Specialization?> UpdateAsync(int id, Specialization specialization);
        Task<bool> DeleteAsync(int id);
    }
} 