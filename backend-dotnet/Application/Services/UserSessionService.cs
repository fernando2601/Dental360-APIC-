using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using DentalSpa.Application.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DentalSpa.Application.Services
{
    public class UserSessionService : IUserSessionService
    {
        private readonly IUserSessionRepository _repository;
        public UserSessionService(IUserSessionRepository repository)
        {
            _repository = repository;
        }
        public Task<IEnumerable<UserSession>> GetAllAsync() => _repository.GetAllAsync();
        public Task<UserSession?> GetByIdAsync(int sessionId) => _repository.GetByIdAsync(sessionId);
        public Task<UserSession> CreateAsync(UserSession session) => _repository.CreateAsync(session);
        public Task<UserSession?> UpdateAsync(int sessionId, UserSession session) => _repository.UpdateAsync(sessionId, session);
        public Task<bool> DeleteAsync(int sessionId) => _repository.DeleteAsync(sessionId);
    }
} 