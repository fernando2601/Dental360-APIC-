using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using DentalSpa.Application.Interfaces;

namespace DentalSpa.Application.Services
{
    public class BeforeAfterService : IBeforeAfterService
    {
        private readonly IBeforeAfterRepository _beforeAfterRepository;
        private readonly ILogger<BeforeAfterService> _logger;

        public BeforeAfterService(
            IBeforeAfterRepository beforeAfterRepository,
            ILogger<BeforeAfterService> logger)
        {
            _beforeAfterRepository = beforeAfterRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<BeforeAfter>> GetAllAsync()
        {
            _logger.LogInformation("Retrieving all before/after cases");
            return await _beforeAfterRepository.GetAllAsync();
        }

        public async Task<BeforeAfter?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Retrieving before/after case with ID: {CaseId}", id);
            if (id <= 0)
                throw new ArgumentException("ID do caso deve ser maior que zero", nameof(id));
            return await _beforeAfterRepository.GetByIdAsync(id);
        }

        public async Task<BeforeAfter> CreateAsync(BeforeAfter beforeAfter)
        {
            _logger.LogInformation("Creating new before/after case");
            return await _beforeAfterRepository.CreateAsync(beforeAfter);
        }

        public async Task<BeforeAfter?> UpdateAsync(int id, BeforeAfter beforeAfter)
        {
            _logger.LogInformation("Updating before/after case with ID: {CaseId}", id);
            if (id <= 0)
                throw new ArgumentException("ID do caso deve ser maior que zero", nameof(id));
            return await _beforeAfterRepository.UpdateAsync(id, beforeAfter);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            _logger.LogInformation("Deleting before/after case with ID: {CaseId}", id);
            if (id <= 0)
                throw new ArgumentException("ID do caso deve ser maior que zero", nameof(id));
            return await _beforeAfterRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<BeforeAfter>> GetPublicAsync()
        {
            _logger.LogInformation("Retrieving public before/after cases");
            return await _beforeAfterRepository.GetPublicAsync();
        }

        public async Task<IEnumerable<BeforeAfter>> GetByServiceAsync(int serviceId)
        {
            _logger.LogInformation("Retrieving before/after cases by service ID: {ServiceId}", serviceId);
            if (serviceId <= 0)
                throw new ArgumentException("ID do serviço deve ser maior que zero", nameof(serviceId));
            return await _beforeAfterRepository.GetByServiceAsync(serviceId);
        }

        public async Task<IEnumerable<BeforeAfter>> SearchAsync(string searchTerm)
        {
            _logger.LogInformation("Searching before/after cases with term: {SearchTerm}", searchTerm);
            return await _beforeAfterRepository.SearchAsync(searchTerm);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _beforeAfterRepository.ExistsAsync(id);
        }
    }
}