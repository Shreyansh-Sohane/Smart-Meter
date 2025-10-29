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
    public class TariffController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TariffController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("Add")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> AddTariff([FromBody] AddTariffRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var tariff = new Tariff
            {
                Name = request.Name,
                Effectivefrom = request.EffectiveFrom,
                Effectiveto = request.EffectiveTo,
                Baserate = request.BaseRate,
                Taxrate = request.TaxRate
            };

            _context.Tariffs.Add(tariff);
            await _context.SaveChangesAsync();

            var response = new TariffResponse
            {
                TariffId = tariff.Tariffid,
                Name = tariff.Name,
                EffectiveFrom = tariff.Effectivefrom,
                EffectiveTo = tariff.Effectiveto,
                BaseRate = tariff.Baserate,
                TaxRate = tariff.Taxrate
            };

            return CreatedAtAction(nameof(GetTariffById), new { id = tariff.Tariffid }, response);
        }

        [HttpGet("GetAll")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> GetAllTariffs()
        {
            var tariffs = await _context.Tariffs
                .Select(t => new TariffResponse
                {
                    TariffId = t.Tariffid,
                    Name = t.Name,
                    EffectiveFrom = t.Effectivefrom,
                    EffectiveTo = t.Effectiveto,
                    BaseRate = t.Baserate,
                    TaxRate = t.Taxrate
                })
                .ToListAsync();

            return Ok(tariffs);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> GetTariffById(int id)
        {
            var tariff = await _context.Tariffs
                .Where(t => t.Tariffid == id)
                .Select(t => new TariffResponse
                {
                    TariffId = t.Tariffid,
                    Name = t.Name,
                    EffectiveFrom = t.Effectivefrom,
                    EffectiveTo = t.Effectiveto,
                    BaseRate = t.Baserate,
                    TaxRate = t.Taxrate
                })
                .FirstOrDefaultAsync();

            if (tariff == null)
                return NotFound($"Tariff with ID {id} not found.");

            return Ok(tariff);
        }

        [HttpPut("Update/{id}")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> UpdateTariff(int id, [FromBody] UpdateTariffRequest request)
        {
            var tariff = await _context.Tariffs.FindAsync(id);
            if (tariff == null)
                return NotFound($"Tariff with ID {id} not found.");

            tariff.Name = request.Name;
            tariff.Effectivefrom = request.EffectiveFrom;
            tariff.Effectiveto = request.EffectiveTo;
            tariff.Baserate = request.BaseRate;
            tariff.Taxrate = request.TaxRate;

            await _context.SaveChangesAsync();

            return Ok(new TariffResponse
            {
                TariffId = tariff.Tariffid,
                Name = tariff.Name,
                EffectiveFrom = tariff.Effectivefrom,
                EffectiveTo = tariff.Effectiveto,
                BaseRate = tariff.Baserate,
                TaxRate = tariff.Taxrate
            });
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> DeleteTariff(int id)
        {
            var tariff = await _context.Tariffs.FindAsync(id);
            if (tariff == null)
                return NotFound($"Tariff with ID {id} not found.");

            _context.Tariffs.Remove(tariff);
            await _context.SaveChangesAsync();

            return Ok($"Tariff with ID {id} deleted successfully.");
        }
    }
}
