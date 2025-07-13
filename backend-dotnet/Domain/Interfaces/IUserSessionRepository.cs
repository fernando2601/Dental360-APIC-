using System.Collections.Generic;
using System.Threading.Tasks;
using DentalSpa.Domain.Entities;

namespace DentalSpa.Domain.Interfaces
{
    public interface IUserSessionRepository
    {
        Task<IEnumerable<UserSession>> GetAllAsync();
        Task<UserSession?> GetByIdAsync(int sessionId);
        Task<UserSession> CreateAsync(UserSession session);
        Task<UserSession?> UpdateAsync(int sessionId, UserSession session);
        Task<bool> DeleteAsync(int sessionId);
    }
} 