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
    public class TodruleController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TodruleController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("Add")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> AddTodrule([FromBody] AddTodruleRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var todrule = new Todrule
            {
                Tariffid = request.TariffId,
                Name = request.Name,
                Starttime = request.StartTime,
                Endtime = request.EndTime,
                Rateperkwh = request.RatePerKwh,
                Deleted = request.Deleted
            };

            _context.Todrules.Add(todrule);
            await _context.SaveChangesAsync();

            var response = new TodruleResponse
            {
                TodruleId = todrule.Todruleid,
                TariffId = todrule.Tariffid,
                Name = todrule.Name,
                StartTime = todrule.Starttime,
                EndTime = todrule.Endtime,
                RatePerKwh = todrule.Rateperkwh,
                Deleted = todrule.Deleted
            };

            return CreatedAtAction(nameof(GetTodruleById), new { id = todrule.Todruleid }, response);
        }

        [HttpGet("GetAll")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> GetAllTodrules()
        {
            var todrules = await _context.Todrules
                .Where(t => !t.Deleted)
                .Select(t => new TodruleResponse
                {
                    TodruleId = t.Todruleid,
                    TariffId = t.Tariffid,
                    Name = t.Name,
                    StartTime = t.Starttime,
                    EndTime = t.Endtime,
                    RatePerKwh = t.Rateperkwh,
                    Deleted = t.Deleted
                })
                .ToListAsync();

            return Ok(todrules);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> GetTodruleById(int id)
        {
            var todrule = await _context.Todrules
                .Where(t => t.Todruleid == id && !t.Deleted)
                .Select(t => new TodruleResponse
                {
                    TodruleId = t.Todruleid,
                    TariffId = t.Tariffid,
                    Name = t.Name,
                    StartTime = t.Starttime,
                    EndTime = t.Endtime,
                    RatePerKwh = t.Rateperkwh,
                    Deleted = t.Deleted
                })
                .FirstOrDefaultAsync();

            if (todrule == null)
                return NotFound($"Todrule with ID {id} not found.");

            return Ok(todrule);
        }

        [HttpPut("Update/{id}")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> UpdateTodrule(int id, [FromBody] UpdateTodruleRequest request)
        {
            var todrule = await _context.Todrules.FindAsync(id);
            if (todrule == null || todrule.Deleted)
                return NotFound($"Todrule with ID {id} not found.");

            todrule.Tariffid = request.TariffId;
            todrule.Name = request.Name;
            todrule.Starttime = request.StartTime;
            todrule.Endtime = request.EndTime;
            todrule.Rateperkwh = request.RatePerKwh;
            todrule.Deleted = request.Deleted;

            await _context.SaveChangesAsync();

            return Ok(new TodruleResponse
            {
                TodruleId = todrule.Todruleid,
                TariffId = todrule.Tariffid,
                Name = todrule.Name,
                StartTime = todrule.Starttime,
                EndTime = todrule.Endtime,
                RatePerKwh = todrule.Rateperkwh,
                Deleted = todrule.Deleted
            });
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> DeleteTodrule(int id)
        {
            var todrule = await _context.Todrules.FindAsync(id);
            if (todrule == null || todrule.Deleted)
                return NotFound($"Todrule with ID {id} not found.");

            // Soft delete
            todrule.Deleted = true;
            await _context.SaveChangesAsync();

            return Ok($"Todrule with ID {id} deleted successfully.");
        }
    }
}
