using DentalSpa.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DentalSpa.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> FindByEmailAsync(string email);
        Task<User?> FindByUsernameAsync(string username);
        Task<User?> FindByRefreshTokenAsync(string refreshToken);
        Task<User?> GetByIdAsync(int id);
        Task<User> CreateAsync(User user);
        Task<User?> UpdateAsync(int id, User user);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<User>> GetAllAsync();
        Task<IEnumerable<User>> SearchAsync(string query);
        Task<object> GetProfileByIdAsync(int id);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
    }
}