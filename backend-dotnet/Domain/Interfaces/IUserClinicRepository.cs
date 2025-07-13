using System.Collections.Generic;
using System.Threading.Tasks;
using DentalSpa.Domain.Entities;

namespace DentalSpa.Domain.Interfaces
{
    public interface IUserClinicRepository
    {
        Task<IEnumerable<UserClinic>> GetAllAsync();
        Task<UserClinic?> GetByIdsAsync(int userId, int clinicId);
        Task<UserClinic> CreateAsync(UserClinic userClinic);
        Task<UserClinic?> UpdateAsync(int userId, int clinicId, UserClinic userClinic);
        Task<bool> DeleteAsync(int userId, int clinicId);
    }
} 