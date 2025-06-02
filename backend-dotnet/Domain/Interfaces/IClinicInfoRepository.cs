using DentalSpa.Domain.Entities;

namespace DentalSpa.Domain.Interfaces
{
    public interface IClinicInfoRepository
    {
        Task<ClinicInfo?> GetClinicInfoAsync();
        Task<ClinicInfo> CreateOrUpdateClinicInfoAsync(ClinicInfo clinicInfo);
        Task<bool> DeleteClinicInfoAsync(int id);
    }
}