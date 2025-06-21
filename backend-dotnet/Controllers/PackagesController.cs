using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DentalSpa.Domain.Entities;
using DentalSpa.Application.Interfaces;

namespace DentalSpa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PackagesController : ControllerBase
    {
        private readonly IPackageService _packageService;

        public PackagesController(IPackageService packageService)
        {
            _packageService = packageService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Package>>> GetAllPackages()
            => Ok(await _packageService.GetAllPackagesAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<Package>> GetPackage(int id)
        {
            var package = await _packageService.GetPackageByIdAsync(id);
            if (package == null) return NotFound();
            return Ok(package);
        }

        [HttpPost]
        public async Task<ActionResult<Package>> CreatePackage([FromBody] Package package)
        {
            var created = await _packageService.CreatePackageAsync(package);
            return CreatedAtAction(nameof(GetPackage), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Package>> UpdatePackage(int id, [FromBody] Package package)
        {
            var updated = await _packageService.UpdatePackageAsync(id, package);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePackage(int id)
        {
            var result = await _packageService.DeletePackageAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<Package>>> GetActivePackages()
            => Ok(await _packageService.GetActivePackagesAsync());

        [HttpGet("popular")]
        public async Task<ActionResult<IEnumerable<Package>>> GetPopularPackages()
            => Ok(await _packageService.GetPopularPackagesAsync());
    }
} 