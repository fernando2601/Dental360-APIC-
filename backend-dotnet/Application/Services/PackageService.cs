using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using DentalSpa.Application.Interfaces;

namespace DentalSpa.Application.Services
{
    public class PackageService : IPackageService
    {
        private readonly IPackageRepository _packageRepository;
        private readonly ILogger<PackageService> _logger;

        public PackageService(IPackageRepository packageRepository, ILogger<PackageService> logger)
        {
            _packageRepository = packageRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Package>> GetAllPackagesAsync()
        {
            try
            {
                return await _packageRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all packages");
                throw;
            }
        }

        public async Task<Package?> GetPackageByIdAsync(int id)
        {
            try
            {
                return await _packageRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving package with ID: {PackageId}", id);
                throw;
            }
        }

        public async Task<Package> CreatePackageAsync(Package package)
        {
            try
            {
                return await _packageRepository.CreateAsync(package);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating package");
                throw;
            }
        }

        public async Task<Package?> UpdatePackageAsync(int id, Package package)
        {
            try
            {
                package.Id = id; // Ensure the package has the correct ID
                return await _packageRepository.UpdateAsync(id, package);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating package with ID: {PackageId}", id);
                throw;
            }
        }

        public async Task<bool> DeletePackageAsync(int id)
        {
            try
            {
                return await _packageRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting package with ID: {PackageId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<Package>> SearchPackagesAsync(string searchTerm)
        {
            try
            {
                return await _packageRepository.SearchAsync(searchTerm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching packages with term: {SearchTerm}", searchTerm);
                throw;
            }
        }

        public async Task<IEnumerable<Package>> GetActivePackagesAsync()
        {
            try
            {
                // Implementação básica - retorna todos os pacotes
                return await _packageRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active packages");
                throw;
            }
        }

        public async Task<IEnumerable<Package>> GetPopularPackagesAsync()
        {
            try
            {
                // Implementação básica - retorna todos os pacotes
                return await _packageRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving popular packages");
                throw;
            }
        }
    }
} 