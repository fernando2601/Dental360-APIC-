using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using DentalSpa.Application.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DentalSpa.Application.Services
{
    public class SpecializationService : ISpecializationService
    {
        private readonly ISpecializationRepository _repository;
        public SpecializationService(ISpecializationRepository repository)
        {
            _repository = repository;
        }
        public Task<IEnumerable<Specialization>> GetAllAsync() => _repository.GetAllAsync();
        public Task<Specialization?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);
        public Task<Specialization> CreateAsync(Specialization specialization) => _repository.CreateAsync(specialization);
        public Task<Specialization?> UpdateAsync(int id, Specialization specialization) => _repository.UpdateAsync(id, specialization);
        public Task<bool> DeleteAsync(int id) => _repository.DeleteAsync(id);
    }
} 