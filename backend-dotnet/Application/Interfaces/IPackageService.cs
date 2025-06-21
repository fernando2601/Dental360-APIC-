using DentalSpa.Domain.Entities;

namespace DentalSpa.Application.Interfaces
{
    public interface IPackageService
    {
        Task<IEnumerable<Package>> GetAllPackagesAsync();
        Task<Package?> GetPackageByIdAsync(int id);
        Task<Package> CreatePackageAsync(Package package);
        Task<Package?> UpdatePackageAsync(int id, Package package);
        Task<bool> DeletePackageAsync(int id);
        Task<IEnumerable<Package>> SearchPackagesAsync(string searchTerm);
        Task<IEnumerable<Package>> GetActivePackagesAsync();
        Task<IEnumerable<Package>> GetPopularPackagesAsync();
    }
} 