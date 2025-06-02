using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DentalSpa.Application.Interfaces;
using DentalSpa.Domain.Entities;

namespace DentalSpa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
        public async Task<ActionResult<ClinicInfo>> GetClinicInfo()
        {
            try
            {
                var clinicInfo = await _clinicInfoService.GetClinicInfoAsync();
                return Ok(clinicInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar informações da clínica");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ClinicInfo>> CreateOrUpdateClinicInfo([FromBody] ClinicInfo clinicInfo)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _clinicInfoService.CreateOrUpdateClinicInfoAsync(clinicInfo);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar/atualizar informações da clínica");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteClinicInfo(int id)
        {
            try
            {
                var success = await _clinicInfoService.DeleteClinicInfoAsync(id);
                if (!success)
                {
                    return NotFound(new { message = "Informações da clínica não encontradas" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar informações da clínica: {Id}", id);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }
    }
}