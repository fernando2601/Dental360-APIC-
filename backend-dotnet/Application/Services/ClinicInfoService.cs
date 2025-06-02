using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using DentalSpa.Application.Interfaces;

namespace DentalSpa.Application.Services
{
    public class ClinicInfoService : IClinicInfoService
    {
        private readonly IClinicInfoRepository _clinicInfoRepository;
        private readonly ILogger<ClinicInfoService> _logger;

        public ClinicInfoService(IClinicInfoRepository clinicInfoRepository, ILogger<ClinicInfoService> logger)
        {
            _clinicInfoRepository = clinicInfoRepository;
            _logger = logger;
        }

        public async Task<ClinicInfo?> GetClinicInfoAsync()
        {
            try
            {
                return await _clinicInfoRepository.GetClinicInfoAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar informações da clínica");
                throw;
            }
        }

        public async Task<ClinicInfo> CreateOrUpdateClinicInfoAsync(ClinicInfo clinicInfo)
        {
            try
            {
                return await _clinicInfoRepository.CreateOrUpdateClinicInfoAsync(clinicInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar/atualizar informações da clínica");
                throw;
            }
        }

        public async Task<bool> DeleteClinicInfoAsync(int id)
        {
            try
            {
                return await _clinicInfoRepository.DeleteClinicInfoAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar informações da clínica: {Id}", id);
                throw;
            }
        }
    }
}