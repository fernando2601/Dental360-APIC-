using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DentalSpa.Domain.Entities;
using DentalSpa.Application.Interfaces;

namespace ClinicApi.Controllers
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
        {
            var packages = await _packageService.GetAllPackagesAsync();
            return Ok(packages);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Package>> GetPackage(int id)
        {
            var package = await _packageService.GetPackageByIdAsync(id);
            if (package == null)
                return NotFound();

            return Ok(package);
        }

        [HttpPost]
        public async Task<ActionResult<Package>> CreatePackage(CreatePackageDto packageDto)
        {
            var package = await _packageService.CreatePackageAsync(packageDto);
            return CreatedAtAction(nameof(GetPackage), new { id = package.Id }, package);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Package>> UpdatePackage(int id, CreatePackageDto packageDto)
        {
            var package = await _packageService.UpdatePackageAsync(id, packageDto);
            if (package == null)
                return NotFound();

            return Ok(package);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePackage(int id)
        {
            var result = await _packageService.DeletePackageAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<Package>>> GetActivePackages()
        {
            var packages = await _packageService.GetActivePackagesAsync();
            return Ok(packages);
        }

        [HttpGet("popular")]
        public async Task<ActionResult<IEnumerable<Package>>> GetPopularPackages()
        {
            var packages = await _packageService.GetPopularPackagesAsync();
            return Ok(packages);
        }

        [HttpPost("{id}/subscribe")]
        public async Task<ActionResult> SubscribeToPackage(int id, [FromBody] SubscriptionDto subscriptionDto)
        {
            var result = await _packageService.SubscribeToPackageAsync(id, subscriptionDto);
            return Ok(result);
        }
    }
}