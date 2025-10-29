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
    public class OrgUnitController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrgUnitController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("Add")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> AddOrgUnit([FromBody] AddOrgUnitRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var orgUnit = new Orgunit
            {
                Type = request.Type,
                Name = request.Name,
                Parentid = request.Parentid
            };

            _context.Orgunits.Add(orgUnit);
            await _context.SaveChangesAsync();

            var response = new OrgUnitResponse
            {
                Orgunitid = orgUnit.Orgunitid,
                Type = orgUnit.Type,
                Name = orgUnit.Name,
                Parentid = orgUnit.Parentid
            };

            return CreatedAtAction(nameof(GetOrgUnitById), new { id = orgUnit.Orgunitid }, response);
        }

        [HttpGet("GetAll")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> GetAllOrgUnits()
        {
            var orgUnits = await _context.Orgunits
                .Select(o => new OrgUnitResponse
                {
                    Orgunitid = o.Orgunitid,
                    Type = o.Type,
                    Name = o.Name,
                    Parentid = o.Parentid
                })
                .ToListAsync();

            return Ok(orgUnits);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> GetOrgUnitById(int id)
        {
            var orgUnit = await _context.Orgunits
                .Where(o => o.Orgunitid == id)
                .Select(o => new OrgUnitResponse
                {
                    Orgunitid = o.Orgunitid,
                    Type = o.Type,
                    Name = o.Name,
                    Parentid = o.Parentid
                })
                .FirstOrDefaultAsync();

            if (orgUnit == null)
                return NotFound($"OrgUnit with ID {id} not found.");

            return Ok(orgUnit);
        }

        [HttpPut("Update/{id}")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> UpdateOrgUnit(int id, [FromBody] UpdateOrgUnitRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var orgUnit = await _context.Orgunits.FindAsync(id);
            if (orgUnit == null)
                return NotFound($"OrgUnit with ID {id} not found.");

            orgUnit.Type = request.Type;
            orgUnit.Name = request.Name;
            orgUnit.Parentid = request.Parentid;

            await _context.SaveChangesAsync();

            var response = new OrgUnitResponse
            {
                Orgunitid = orgUnit.Orgunitid,
                Type = orgUnit.Type,
                Name = orgUnit.Name,
                Parentid = orgUnit.Parentid
            };

            return Ok(response);
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> DeleteOrgUnit(int id)
        {
            var orgUnit = await _context.Orgunits.FindAsync(id);
            if (orgUnit == null)
                return NotFound($"OrgUnit with ID {id} not found.");

            _context.Orgunits.Remove(orgUnit);
            await _context.SaveChangesAsync();

            return Ok($"OrgUnit with ID {id} deleted successfully.");
        }
    }
}
