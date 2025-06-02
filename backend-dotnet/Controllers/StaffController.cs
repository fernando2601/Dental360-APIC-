using Microsoft.AspNetCore.Mvc;
using DentalSpa.Domain.Entities;
using DentalSpa.Application.Interfaces;

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
        public async Task<ActionResult<StaffResponse>> CreateStaff([FromBody] CreateStaffRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var staff = await _staffService.CreateStaffAsync(request);
                return CreatedAtAction(nameof(GetStaffById), new { id = staff.Id }, staff);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar funcionário");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<StaffResponse>> UpdateStaff(int id, [FromBody] UpdateStaffRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var staff = await _staffService.UpdateStaffAsync(id, request);
                if (staff == null)
                {
                    return NotFound(new { message = "Funcionário não encontrado" });
                }

                return Ok(staff);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar funcionário {StaffId}", id);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
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
        public async Task<ActionResult<IEnumerable<StaffResponse>>> GetStaffByDepartment(string department)
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
        public async Task<ActionResult<IEnumerable<StaffResponse>>> GetStaffByPosition(string position)
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

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<StaffResponse>>> SearchStaff([FromQuery] string term)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(term))
                {
                    return BadRequest(new { message = "Termo de busca é obrigatório" });
                }

                var staff = await _staffService.SearchStaffAsync(term);
                return Ok(staff);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar funcionários com termo {SearchTerm}", term);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpGet("stats")]
        public async Task<ActionResult<StaffStatsResponse>> GetStaffStats()
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
        public async Task<ActionResult<IEnumerable<StaffResponse>>> GetTeamMembers(int managerId)
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