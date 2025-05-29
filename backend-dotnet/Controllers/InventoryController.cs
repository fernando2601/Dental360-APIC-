using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ClinicApi.Models;
using ClinicApi.Services;

namespace ClinicApi.Controllers
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
        public async Task<ActionResult<IEnumerable<Inventory>>> GetAllInventory()
        {
            var inventory = await _inventoryService.GetAllInventoryAsync();
            return Ok(inventory);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Inventory>> GetInventoryItem(int id)
        {
            var item = await _inventoryService.GetInventoryByIdAsync(id);
            if (item == null)
                return NotFound();

            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<Inventory>> CreateInventoryItem(CreateInventoryDto inventoryDto)
        {
            var item = await _inventoryService.CreateInventoryAsync(inventoryDto);
            return CreatedAtAction(nameof(GetInventoryItem), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Inventory>> UpdateInventoryItem(int id, CreateInventoryDto inventoryDto)
        {
            var item = await _inventoryService.UpdateInventoryAsync(id, inventoryDto);
            if (item == null)
                return NotFound();

            return Ok(item);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInventoryItem(int id)
        {
            var result = await _inventoryService.DeleteInventoryAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpGet("low-stock")]
        public async Task<ActionResult<IEnumerable<Inventory>>> GetLowStockItems()
        {
            var items = await _inventoryService.GetLowStockItemsAsync();
            return Ok(items);
        }

        [HttpGet("expiring")]
        public async Task<ActionResult<IEnumerable<Inventory>>> GetExpiringItems([FromQuery] int days = 30)
        {
            var items = await _inventoryService.GetExpiringItemsAsync(days);
            return Ok(items);
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<Inventory>>> GetInventoryByCategory(string category)
        {
            var items = await _inventoryService.GetInventoryByCategoryAsync(category);
            return Ok(items);
        }
    }
}