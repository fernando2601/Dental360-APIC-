using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DentalSpa.Domain.Entities;
using DentalSpa.Application.Interfaces;

namespace DentalSpa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Inventory>>> GetAll()
            => Ok(await _inventoryService.GetAllInventoryAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<Inventory>> GetById(int id)
        {
            var item = await _inventoryService.GetInventoryByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<Inventory>> Create([FromBody] Inventory inventory)
        {
            var created = await _inventoryService.CreateInventoryAsync(inventory);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Inventory>> Update(int id, [FromBody] Inventory inventory)
        {
            var updated = await _inventoryService.UpdateInventoryAsync(inventory);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _inventoryService.DeleteInventoryAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Inventory>>> Search([FromQuery] string searchTerm)
            => Ok(await _inventoryService.SearchByNameAsync(searchTerm));

        [HttpGet("low-stock")]
        public async Task<ActionResult<IEnumerable<Inventory>>> GetLowStock([FromQuery] int threshold = 10)
            => Ok(await _inventoryService.GetLowStockItemsAsync(threshold));
    }
} 