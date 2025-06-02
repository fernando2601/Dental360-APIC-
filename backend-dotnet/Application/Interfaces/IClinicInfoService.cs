using DentalSpa.Domain.Entities;

namespace DentalSpa.Application.Interfaces
{
    public interface IClinicInfoService
    {
        Task<ClinicInfo?> GetClinicInfoAsync();
        Task<ClinicInfo> CreateOrUpdateClinicInfoAsync(ClinicInfo clinicInfo);
        Task<bool> DeleteClinicInfoAsync(int id);
    }
}