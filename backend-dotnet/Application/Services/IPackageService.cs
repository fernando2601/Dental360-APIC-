using ClinicApi.Models;

namespace ClinicApi.Services
{
    public interface IPackageService
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
        Task<(bool IsValid, string ErrorMessage)> ValidatePackageAsync(CreatePackageRequest request, int? excludeId = null);
    }

    public interface IClinicInfoService
    {
        Task<ClinicInfoResponse?> GetClinicInfoAsync();
        Task<ClinicInfoResponse> UpdateClinicInfoAsync(UpdateClinicInfoRequest request);
        Task<ClinicStatsResponse> GetClinicStatsAsync();
        Task<(bool IsValid, string ErrorMessage)> ValidateClinicInfoAsync(UpdateClinicInfoRequest request);
    }
}