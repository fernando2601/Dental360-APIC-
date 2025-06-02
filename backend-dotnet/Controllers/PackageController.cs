using Microsoft.AspNetCore.Mvc;
using DentalSpa.Domain.Entities;
using DentalSpa.Application.Interfaces;

namespace ClinicApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PackageController : ControllerBase
    {
        private readonly IPackageService _packageService;
        private readonly ILogger<PackageController> _logger;

        public PackageController(IPackageService packageService, ILogger<PackageController> logger)
        {
            _packageService = packageService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PackageResponse>>> GetAllPackages()
        {
            try
            {
                var packages = await _packageService.GetAllPackagesAsync();
                return Ok(packages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving packages");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PackageDetailedModel>> GetPackageById(int id)
        {
            try
            {
                var package = await _packageService.GetPackageByIdAsync(id);
                if (package == null)
                {
                    return NotFound(new { message = "Pacote não encontrado" });
                }

                return Ok(package);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving package with ID: {PackageId}", id);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpPost]
        public async Task<ActionResult<PackageResponse>> CreatePackage([FromBody] CreatePackageRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var package = await _packageService.CreatePackageAsync(request);
                return CreatedAtAction(nameof(GetPackageById), new { id = package.Id }, package);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating package");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PackageResponse>> UpdatePackage(int id, [FromBody] UpdatePackageRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var package = await _packageService.UpdatePackageAsync(id, request);
                if (package == null)
                {
                    return NotFound(new { message = "Pacote não encontrado" });
                }

                return Ok(package);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating package with ID: {PackageId}", id);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePackage(int id)
        {
            try
            {
                var success = await _packageService.DeletePackageAsync(id);
                if (!success)
                {
                    return NotFound(new { message = "Pacote não encontrado" });
                }

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting package with ID: {PackageId}", id);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpGet("stats")]
        public async Task<ActionResult<PackageStatsResponse>> GetPackageStats()
        {
            try
            {
                var stats = await _packageService.GetPackageStatsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving package statistics");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<PackageResponse>>> GetPackagesByCategory(string category)
        {
            try
            {
                var packages = await _packageService.GetPackagesByCategoryAsync(category);
                return Ok(packages);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving packages by category: {Category}", category);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<PackageResponse>>> SearchPackages([FromQuery] string term)
        {
            try
            {
                var packages = await _packageService.SearchPackagesAsync(term ?? string.Empty);
                return Ok(packages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching packages with term: {SearchTerm}", term);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<string>>> GetPackageCategories()
        {
            try
            {
                var categories = await _packageService.GetPackageCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving package categories");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }
    }

    [ApiController]
    [Route("api/clinic-info")]
    public class ClinicInfoController : ControllerBase
    {
        private readonly IClinicInfoService _clinicInfoService;
        private readonly ILogger<ClinicInfoController> _logger;

        public ClinicInfoController(IClinicInfoService clinicInfoService, ILogger<ClinicInfoController> logger)
        {
            _clinicInfoService = clinicInfoService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ClinicInfoResponse>> GetClinicInfo()
        {
            try
            {
                var clinicInfo = await _clinicInfoService.GetClinicInfoAsync();
                if (clinicInfo == null)
                {
                    return NotFound(new { message = "Informações da clínica não encontradas" });
                }

                return Ok(clinicInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving clinic information");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpPut]
        public async Task<ActionResult<ClinicInfoResponse>> UpdateClinicInfo([FromBody] UpdateClinicInfoRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var clinicInfo = await _clinicInfoService.UpdateClinicInfoAsync(request);
                return Ok(clinicInfo);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating clinic information");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpGet("stats")]
        public async Task<ActionResult<ClinicStatsResponse>> GetClinicStats()
        {
            try
            {
                var stats = await _clinicInfoService.GetClinicStatsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving clinic statistics");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }
    }
}