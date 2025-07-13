using DentalSpa.Domain.Entities;
using DentalSpa.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DentalSpa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _service;
        public DepartmentController(IDepartmentService service)
        {
            _service = service;
        }
        [HttpGet]
        public async Task<IEnumerable<Department>> GetAll() => await _service.GetAllAsync();
        [HttpGet("{id}")]
        public async Task<ActionResult<Department>> GetById(int id)
        {
            var department = await _service.GetByIdAsync(id);
            if (department == null) return NotFound();
            return department;
        }
        [HttpPost]
        public async Task<ActionResult<Department>> Create(Department department)
        {
            var created = await _service.CreateAsync(department);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<Department>> Update(int id, Department department)
        {
            var updated = await _service.UpdateAsync(id, department);
            if (updated == null) return NotFound();
            return updated;
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
} 