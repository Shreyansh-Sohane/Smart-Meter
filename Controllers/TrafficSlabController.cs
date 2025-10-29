using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartMeterBackend.Data.Context;
using SmartMeterBackend.Data.Entities;
using SmartMeterBackend.Models.DTOs;

namespace SmartMeterBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TariffSlabController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TariffSlabController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("Add")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> AddTariffSlab([FromBody] AddTariffslabRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var slab = new Tariffslab
            {
                Tariffid = request.Tariffid,
                Fromkwh = request.Fromkwh,
                Tokwh = request.Tokwh,
                Rateperkwh = request.Rateperkwh,
                Deleted = request.Deleted
            };

            _context.Tariffslabs.Add(slab);
            await _context.SaveChangesAsync();

            var response = new TariffslabResponse
            {
                Tariffslabid = slab.Tariffslabid,
                Tariffid = slab.Tariffid,
                Fromkwh = slab.Fromkwh,
                Tokwh = slab.Tokwh,
                Rateperkwh = slab.Rateperkwh,
                Deleted = slab.Deleted
            };

            return CreatedAtAction(nameof(GetTariffSlabById), new { id = slab.Tariffslabid }, response);
        }

        [HttpGet("GetAll")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> GetAllTariffSlabs()
        {
            var slabs = await _context.Tariffslabs
                .Where(s => !s.Deleted)
                .Select(s => new TariffslabResponse
                {
                    Tariffslabid = s.Tariffslabid,
                    Tariffid = s.Tariffid,
                    Fromkwh = s.Fromkwh,
                    Tokwh = s.Tokwh,
                    Rateperkwh = s.Rateperkwh,
                    Deleted = s.Deleted
                })
                .ToListAsync();

            return Ok(slabs);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> GetTariffSlabById(int id)
        {
            var slab = await _context.Tariffslabs
                .Where(s => s.Tariffslabid == id && !s.Deleted)
                .Select(s => new TariffslabResponse
                {
                    Tariffslabid = s.Tariffslabid,
                    Tariffid = s.Tariffid,
                    Fromkwh = s.Fromkwh,
                    Tokwh = s.Tokwh,
                    Rateperkwh = s.Rateperkwh,
                    Deleted = s.Deleted
                })
                .FirstOrDefaultAsync();

            if (slab == null)
                return NotFound($"Tariff slab with ID {id} not found.");

            return Ok(slab);
        }

        [HttpPut("Update/{id}")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> UpdateTariffSlab(int id, [FromBody] UpdateTariffslabRequest request)
        {
            var slab = await _context.Tariffslabs.FindAsync(id);
            if (slab == null || slab.Deleted)
                return NotFound($"Tariff slab with ID {id} not found.");

            slab.Tariffid = request.Tariffid;
            slab.Fromkwh = request.Fromkwh;
            slab.Tokwh = request.Tokwh;
            slab.Rateperkwh = request.Rateperkwh;
            slab.Deleted = request.Deleted;

            await _context.SaveChangesAsync();

            return Ok(new TariffslabResponse
            {
                Tariffslabid = slab.Tariffslabid,
                Tariffid = slab.Tariffid,
                Fromkwh = slab.Fromkwh,
                Tokwh = slab.Tokwh,
                Rateperkwh = slab.Rateperkwh,
                Deleted = slab.Deleted
            });
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> DeleteTariffSlab(int id)
        {
            var slab = await _context.Tariffslabs.FindAsync(id);
            if (slab == null || slab.Deleted)
                return NotFound($"Tariff slab with ID {id} not found.");

            // Soft delete
            slab.Deleted = true;
            await _context.SaveChangesAsync();

            return Ok($"Tariff slab with ID {id} deleted successfully.");
        }
    }
}
