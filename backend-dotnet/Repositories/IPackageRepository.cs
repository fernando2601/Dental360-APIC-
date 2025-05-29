using ClinicApi.Models;

namespace ClinicApi.Repositories
{
    public interface IPackageRepository
    {
        Task<IEnumerable<PackageResponse>> GetAllPackagesAsync();
        Task<PackageDetailedModel?> GetPackageByIdAsync(int id);
        Task<PackageResponse> CreatePackageAsync(CreatePackageRequest request);
        Task<PackageResponse?> UpdatePackageAsync(int id, UpdatePackageRequest request);
        Task<bool> DeletePackageAsync(int id);
        Task<PackageStatsResponse> GetPackageStatsAsync();
        Task<IEnumerable<PackageResponse>> GetPackagesByCategoryAsync(string category);
        Task<IEnumerable<PackageResponse>> SearchPackagesAsync(string searchTerm);
        Task<IEnumerable<string>> GetPackageCategoriesAsync();
        Task<bool> PackageExistsAsync(int id);
        Task<bool> PackageNameExistsAsync(string name, int? excludeId = null);
    }

    public interface IClinicInfoRepository
    {
        Task<ClinicInfoResponse?> GetClinicInfoAsync();
        Task<ClinicInfoResponse> UpdateClinicInfoAsync(UpdateClinicInfoRequest request);
        Task<ClinicStatsResponse> GetClinicStatsAsync();
        Task<bool> ClinicInfoExistsAsync();
        Task<ClinicInfoResponse> CreateDefaultClinicInfoAsync();
    }
}