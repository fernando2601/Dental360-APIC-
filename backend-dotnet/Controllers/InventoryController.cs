using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DentalSpa.Domain.Entities;
using DentalSpa.Application.Interfaces;
using DentalSpa.Application.DTOs;
using DentalSpa.Domain.Interfaces;

namespace DentalSpa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;
        private readonly IProductRepository _productRepository;

        public InventoryController(IInventoryService inventoryService, IProductRepository productRepository)
        {
            _inventoryService = inventoryService;
            _productRepository = productRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryResponse>>> GetAll()
        {
            var items = await _inventoryService.GetAllAsync();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InventoryResponse>> GetById(int id)
        {
            var item = await _inventoryService.GetByIdAsync(id);
            if (item == null)
                return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<InventoryResponse>> Create([FromBody] InventoryCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var response = await _inventoryService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), null, response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<InventoryResponse>> Update(int id, [FromBody] InventoryCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var response = await _inventoryService.UpdateAsync(id, request);
            if (response == null)
                return NotFound();
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _inventoryService.DeleteInventoryAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        [HttpGet("low-stock")]
        public async Task<ActionResult<IEnumerable<InventoryResponse>>> GetLowStock([FromQuery] int threshold = 10)
            => Ok(await _inventoryService.GetLowStockItemsAsync(threshold));
    }
} 