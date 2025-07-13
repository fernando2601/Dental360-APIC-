using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using DentalSpa.Application.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DentalSpa.Application.Services
{
    public class UserClinicService : IUserClinicService
    {
        private readonly IUserClinicRepository _repository;
        public UserClinicService(IUserClinicRepository repository)
        {
            _repository = repository;
        }
        public Task<IEnumerable<UserClinic>> GetAllAsync() => _repository.GetAllAsync();
        public Task<UserClinic?> GetByIdsAsync(int userId, int clinicId) => _repository.GetByIdsAsync(userId, clinicId);
        public Task<UserClinic> CreateAsync(UserClinic userClinic) => _repository.CreateAsync(userClinic);
        public Task<UserClinic?> UpdateAsync(int userId, int clinicId, UserClinic userClinic) => _repository.UpdateAsync(userId, clinicId, userClinic);
        public Task<bool> DeleteAsync(int userId, int clinicId) => _repository.DeleteAsync(userId, clinicId);
    }
} 