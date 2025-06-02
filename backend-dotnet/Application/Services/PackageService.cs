using ClinicApi.Models;
using ClinicApi.Repositories;

namespace ClinicApi.Services
{
    public class PackageService : IPackageService
    {
        private readonly IPackageRepository _packageRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly ILogger<PackageService> _logger;

        public PackageService(
            IPackageRepository packageRepository,
            IServiceRepository serviceRepository,
            ILogger<PackageService> logger)
        {
            _packageRepository = packageRepository;
            _serviceRepository = serviceRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<PackageResponse>> GetAllPackagesAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving all packages");
                return await _packageRepository.GetAllPackagesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving packages");
                throw new ApplicationException("Erro ao buscar pacotes", ex);
            }
        }

        public async Task<PackageDetailedModel?> GetPackageByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Retrieving package with ID: {PackageId}", id);
                
                if (id <= 0)
                {
                    throw new ArgumentException("ID do pacote deve ser maior que zero", nameof(id));
                }

                return await _packageRepository.GetPackageByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving package with ID: {PackageId}", id);
                throw new ApplicationException($"Erro ao buscar pacote com ID {id}", ex);
            }
        }

        public async Task<PackageResponse> CreatePackageAsync(CreatePackageRequest request)
        {
            try
            {
                _logger.LogInformation("Creating new package: {PackageName}", request.Name);

                var (isValid, errorMessage) = await ValidatePackageAsync(request);
                if (!isValid)
                {
                    throw new ArgumentException(errorMessage);
                }

                // Validate services exist
                await ValidateServicesExistAsync(request.ServiceIds);

                return await _packageRepository.CreatePackageAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating package: {PackageName}", request.Name);
                
                if (ex is ArgumentException)
                    throw;
                    
                throw new ApplicationException("Erro ao criar pacote", ex);
            }
        }

        public async Task<PackageResponse?> UpdatePackageAsync(int id, UpdatePackageRequest request)
        {
            try
            {
                _logger.LogInformation("Updating package with ID: {PackageId}", id);

                if (id <= 0)
                {
                    throw new ArgumentException("ID do pacote deve ser maior que zero", nameof(id));
                }

                var existingPackage = await _packageRepository.GetPackageByIdAsync(id);
                if (existingPackage == null)
                {
                    throw new KeyNotFoundException($"Pacote com ID {id} não encontrado");
                }

                var (isValid, errorMessage) = await ValidatePackageAsync(request, id);
                if (!isValid)
                {
                    throw new ArgumentException(errorMessage);
                }

                // Validate services exist
                await ValidateServicesExistAsync(request.ServiceIds);

                return await _packageRepository.UpdatePackageAsync(id, request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating package with ID: {PackageId}", id);
                
                if (ex is ArgumentException || ex is KeyNotFoundException)
                    throw;
                    
                throw new ApplicationException($"Erro ao atualizar pacote com ID {id}", ex);
            }
        }

        public async Task<bool> DeletePackageAsync(int id)
        {
            try
            {
                _logger.LogInformation("Deleting package with ID: {PackageId}", id);

                if (id <= 0)
                {
                    throw new ArgumentException("ID do pacote deve ser maior que zero", nameof(id));
                }

                var existingPackage = await _packageRepository.GetPackageByIdAsync(id);
                if (existingPackage == null)
                {
                    throw new KeyNotFoundException($"Pacote com ID {id} não encontrado");
                }

                return await _packageRepository.DeletePackageAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting package with ID: {PackageId}", id);
                
                if (ex is ArgumentException || ex is KeyNotFoundException)
                    throw;
                    
                throw new ApplicationException($"Erro ao excluir pacote com ID {id}", ex);
            }
        }

        public async Task<PackageStatsResponse> GetPackageStatsAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving package statistics");
                return await _packageRepository.GetPackageStatsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving package statistics");
                throw new ApplicationException("Erro ao buscar estatísticas de pacotes", ex);
            }
        }

        public async Task<IEnumerable<PackageResponse>> GetPackagesByCategoryAsync(string category)
        {
            try
            {
                _logger.LogInformation("Retrieving packages by category: {Category}", category);

                if (string.IsNullOrWhiteSpace(category))
                {
                    throw new ArgumentException("Categoria não pode ser vazia", nameof(category));
                }

                return await _packageRepository.GetPackagesByCategoryAsync(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving packages by category: {Category}", category);
                
                if (ex is ArgumentException)
                    throw;
                    
                throw new ApplicationException($"Erro ao buscar pacotes da categoria {category}", ex);
            }
        }

        public async Task<IEnumerable<PackageResponse>> SearchPackagesAsync(string searchTerm)
        {
            try
            {
                _logger.LogInformation("Searching packages with term: {SearchTerm}", searchTerm);

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return await GetAllPackagesAsync();
                }

                return await _packageRepository.SearchPackagesAsync(searchTerm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching packages with term: {SearchTerm}", searchTerm);
                throw new ApplicationException($"Erro ao buscar pacotes com termo '{searchTerm}'", ex);
            }
        }

        public async Task<IEnumerable<string>> GetPackageCategoriesAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving package categories");
                return await _packageRepository.GetPackageCategoriesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving package categories");
                throw new ApplicationException("Erro ao buscar categorias de pacotes", ex);
            }
        }

        public async Task<(bool IsValid, string ErrorMessage)> ValidatePackageAsync(CreatePackageRequest request, int? excludeId = null)
        {
            // Validate name uniqueness
            if (await _packageRepository.PackageNameExistsAsync(request.Name, excludeId))
            {
                return (false, "Já existe um pacote com este nome");
            }

            // Validate price consistency
            if (request.DiscountPercentage > 0)
            {
                var expectedFinalPrice = request.OriginalPrice * (1 - request.DiscountPercentage / 100);
                if (Math.Abs(expectedFinalPrice - (request.OriginalPrice * (1 - request.DiscountPercentage / 100))) > 0.01m)
                {
                    return (false, "Preço final não está consistente com o desconto aplicado");
                }
            }

            // Validate service IDs
            if (!request.ServiceIds.Any())
            {
                return (false, "Pacote deve incluir pelo menos um serviço");
            }

            // Check for duplicate service IDs
            if (request.ServiceIds.Count != request.ServiceIds.Distinct().Count())
            {
                return (false, "Não é possível incluir o mesmo serviço múltiplas vezes");
            }

            // Validate duration and validity
            if (request.DurationDays > request.ValidityDays)
            {
                return (false, "Duração do pacote não pode ser maior que a validade");
            }

            return (true, string.Empty);
        }

        private async Task ValidateServicesExistAsync(List<int> serviceIds)
        {
            foreach (var serviceId in serviceIds)
            {
                var service = await _serviceRepository.GetServiceByIdAsync(serviceId);
                if (service == null)
                {
                    throw new ArgumentException($"Serviço com ID {serviceId} não encontrado");
                }

                if (!service.IsActive)
                {
                    throw new ArgumentException($"Serviço '{service.Name}' está inativo e não pode ser incluído no pacote");
                }
            }
        }
    }

    public class ClinicInfoService : IClinicInfoService
    {
        private readonly IClinicInfoRepository _clinicInfoRepository;
        private readonly ILogger<ClinicInfoService> _logger;

        public ClinicInfoService(
            IClinicInfoRepository clinicInfoRepository,
            ILogger<ClinicInfoService> logger)
        {
            _clinicInfoRepository = clinicInfoRepository;
            _logger = logger;
        }

        public async Task<ClinicInfoResponse?> GetClinicInfoAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving clinic information");
                return await _clinicInfoRepository.GetClinicInfoAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving clinic information");
                throw new ApplicationException("Erro ao buscar informações da clínica", ex);
            }
        }

        public async Task<ClinicInfoResponse> UpdateClinicInfoAsync(UpdateClinicInfoRequest request)
        {
            try
            {
                _logger.LogInformation("Updating clinic information");

                var (isValid, errorMessage) = await ValidateClinicInfoAsync(request);
                if (!isValid)
                {
                    throw new ArgumentException(errorMessage);
                }

                return await _clinicInfoRepository.UpdateClinicInfoAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating clinic information");
                
                if (ex is ArgumentException)
                    throw;
                    
                throw new ApplicationException("Erro ao atualizar informações da clínica", ex);
            }
        }

        public async Task<ClinicStatsResponse> GetClinicStatsAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving clinic statistics");
                return await _clinicInfoRepository.GetClinicStatsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving clinic statistics");
                throw new ApplicationException("Erro ao buscar estatísticas da clínica", ex);
            }
        }

        public async Task<(bool IsValid, string ErrorMessage)> ValidateClinicInfoAsync(UpdateClinicInfoRequest request)
        {
            // Validate email format (additional validation beyond model annotation)
            if (!string.IsNullOrEmpty(request.Email) && !IsValidEmail(request.Email))
            {
                return (false, "Formato de email inválido");
            }

            // Validate website URL if provided
            if (!string.IsNullOrEmpty(request.Website) && !IsValidUrl(request.Website))
            {
                return (false, "URL do website inválida");
            }

            // Validate phone format (basic Brazilian phone validation)
            if (!string.IsNullOrEmpty(request.Phone) && !IsValidBrazilianPhone(request.Phone))
            {
                return (false, "Formato de telefone inválido. Use o formato: (XX) XXXXX-XXXX");
            }

            return (true, string.Empty);
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private static bool IsValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var result) 
                   && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
        }

        private static bool IsValidBrazilianPhone(string phone)
        {
            // Remove all non-digit characters
            var digitsOnly = new string(phone.Where(char.IsDigit).ToArray());
            
            // Brazilian phone should have 10 or 11 digits
            return digitsOnly.Length >= 10 && digitsOnly.Length <= 11;
        }
    }
}