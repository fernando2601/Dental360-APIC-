using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ClinicApi.Models;
using ClinicApi.Services;

namespace ClinicApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _staffService;

        public StaffController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Staff>>> GetAllStaff()
        {
            var staff = await _staffService.GetAllStaffAsync();
            return Ok(staff);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Staff>> GetStaff(int id)
        {
            var staff = await _staffService.GetStaffByIdAsync(id);
            if (staff == null)
                return NotFound();

            return Ok(staff);
        }

        [HttpPost]
        public async Task<ActionResult<Staff>> CreateStaff(CreateStaffDto staffDto)
        {
            var staff = await _staffService.CreateStaffAsync(staffDto);
            return CreatedAtAction(nameof(GetStaff), new { id = staff.Id }, staff);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Staff>> UpdateStaff(int id, CreateStaffDto staffDto)
        {
            var staff = await _staffService.UpdateStaffAsync(id, staffDto);
            if (staff == null)
                return NotFound();

            return Ok(staff);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStaff(int id)
        {
            var result = await _staffService.DeleteStaffAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Staff>>> SearchStaff([FromQuery] string term)
        {
            var staff = await _staffService.SearchStaffAsync(term);
            return Ok(staff);
        }

        [HttpGet("specialization/{specialization}")]
        public async Task<ActionResult<IEnumerable<Staff>>> GetStaffBySpecialization(string specialization)
        {
            var staff = await _staffService.GetStaffBySpecializationAsync(specialization);
            return Ok(staff);
        }
    }
}