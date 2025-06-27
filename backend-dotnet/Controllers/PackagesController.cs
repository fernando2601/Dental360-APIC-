using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DentalSpa.Domain.Entities;
using DentalSpa.Application.Interfaces;

namespace DentalSpa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAllProducts()
            => Ok(await _productService.GetAllProductsAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            var created = await _productService.CreateProductAsync(product);
            return CreatedAtAction(nameof(GetProduct), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Product>> UpdateProduct(int id, [FromBody] Product product)
        {
            var updated = await _productService.UpdateProductAsync(id, product);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProductAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<Product>>> GetActiveProducts()
            => Ok(await _productService.GetActiveProductsAsync());

        [HttpGet("popular")]
        public async Task<ActionResult<IEnumerable<Product>>> GetPopularProducts()
            => Ok(await _productService.GetPopularProductsAsync());
    }
} 