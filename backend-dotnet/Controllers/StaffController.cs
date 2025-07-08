using Microsoft.AspNetCore.Mvc;
using DentalSpa.Domain.Entities;
using DentalSpa.Application.Interfaces;
using DentalSpa.Application.DTOs;

namespace DentalSpa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _staffService;
        private readonly ILogger<StaffController> _logger;

        public StaffController(IStaffService staffService, ILogger<StaffController> logger)
        {
            _staffService = staffService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StaffResponse>>> GetAllStaff()
        {
            try
            {
                var staff = await _staffService.GetAllStaffAsync();
                return Ok(staff);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar funcionários");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StaffResponse>> GetStaffById(int id)
        {
            try
            {
                var staff = await _staffService.GetStaffByIdAsync(id);
                if (staff == null)
                {
                    return NotFound(new { message = "Funcionário não encontrado" });
                }

                return Ok(staff);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar funcionário {StaffId}", id);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpPost]
        public async Task<ActionResult<StaffResponse>> Create([FromBody] StaffCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var response = await _staffService.CreateAsync(request);
            return CreatedAtAction(nameof(GetStaffById), null, response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<StaffResponse>> Update(int id, [FromBody] StaffCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var response = await _staffService.UpdateAsync(id, request);
            if (response == null)
                return NotFound();
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteStaff(int id)
        {
            try
            {
                var deleted = await _staffService.DeleteStaffAsync(id);
                if (!deleted)
                {
                    return NotFound(new { message = "Funcionário não encontrado" });
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir funcionário {StaffId}", id);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpGet("department/{department}")]
        public async Task<ActionResult<IEnumerable<Staff>>> GetStaffByDepartment(string department)
        {
            try
            {
                var staff = await _staffService.GetStaffByDepartmentAsync(department);
                return Ok(staff);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar funcionários do departamento {Department}", department);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpGet("position/{position}")]
        public async Task<ActionResult<IEnumerable<Staff>>> GetStaffByPosition(string position)
        {
            try
            {
                var staff = await _staffService.GetStaffByPositionAsync(position);
                return Ok(staff);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar funcionários do cargo {Position}", position);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpGet("stats")]
        public async Task<ActionResult<object>> GetStaffStats()
        {
            try
            {
                var stats = await _staffService.GetStaffStatsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar estatísticas de funcionários");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpGet("departments")]
        public async Task<ActionResult<IEnumerable<string>>> GetDepartments()
        {
            try
            {
                var departments = await _staffService.GetDepartmentsAsync();
                return Ok(departments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar departamentos");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpGet("positions")]
        public async Task<ActionResult<IEnumerable<string>>> GetPositions()
        {
            try
            {
                var positions = await _staffService.GetPositionsAsync();
                return Ok(positions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar cargos");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpGet("{managerId}/team")]
        public async Task<ActionResult<IEnumerable<Staff>>> GetTeamMembers(int managerId)
        {
            try
            {
                var teamMembers = await _staffService.GetTeamMembersAsync(managerId);
                return Ok(teamMembers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar membros da equipe do gerente {ManagerId}", managerId);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }
    }
}