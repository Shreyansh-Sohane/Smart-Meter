using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartMeterBackend.Data.Context;
using SmartMeterBackend.Data.Entities;
using SmartMeterBackend.Models.DTOs;
using System.Security.Claims;
using System.Text;

namespace SmartMeterBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsumerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ConsumerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Add a new consumer
        [HttpPost("AddConsumer")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> AddConsumer([FromBody] AddConsumerRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if phone number already exists
            var exists = await _context.Consumers.AnyAsync(c => c.Phone == request.Phone);
            if (exists)
                return BadRequest(new { message = "Consumer already exists with this mobile number." });

            // Generate random password
            string generatedPassword = GenerateRandomPassword(8);
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(generatedPassword);
            // Fetch orgunit by primary key

            var orgunit = await _context.Orgunits.FindAsync(request.Orgunitid);

            if (orgunit == null)
                return BadRequest(new { message = "Orgunit not found." });

            // Validate orgunit type
            if (orgunit.Type != "DTR" && orgunit.Type != "Feeder")
                return BadRequest(new { message = "Orgunit can only be 'Feeder' or 'DTR'." });


            var newConsumer = new Consumer
            {
                Name = request.Name,
                Phone = request.Phone,
                Email = request.Email,
                Orgunitid = request.Orgunitid,
                Tariffid = request.Tariffid,
                Status = request.Status,
                Createdby = User.FindFirstValue(ClaimTypes.NameIdentifier),
                Deleted = false,
                Passwordhash = Encoding.UTF8.GetBytes(hashedPassword), 
            };

            _context.Consumers.Add(newConsumer);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Consumer added successfully.",
                consumerId = newConsumer.Consumerid,
                generatedPassword
            });
        }

        // Get all consumers
        [HttpGet("GetConsumers")]
        [Authorize(Roles = "user,admin")]
        public async Task<IActionResult> GetConsumers()
        {
            var consumers = await _context.Consumers
                .Where(c => !c.Deleted)
                .ToListAsync();

            return Ok(consumers);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "user,admin")]
        public async Task<IActionResult> GetConsumer(int id)
        {
            var consumer = await _context.Consumers.FindAsync(id);

            if (consumer == null || consumer.Deleted)
                return NotFound(new { message = "Consumer not found." });

            return Ok(consumer);
        }

        [HttpPut("UpdateConsumer/{id}")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> UpdateConsumer(int id, [FromBody] UpdateConsumerRequest request)
        {
            var consumer = await _context.Consumers.FindAsync(id);

            if (consumer == null || consumer.Deleted)
                return NotFound(new { message = "Consumer not found." });

            consumer.Name = request.Name;
            consumer.Phone = request.Phone;
            consumer.Email = request.Email;
            consumer.Orgunitid = request.Orgunitid;
            consumer.Tariffid = request.Tariffid;
            consumer.Status = request.Status;
            consumer.Createdat = request.Createdat;
            consumer.Deleted = request.Deleted;

            _context.Consumers.Update(consumer);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Consumer updated successfully." });
        }

        [HttpDelete("DeleteConsumer/{id}")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> DeleteConsumer(int id)
        {
            var consumer = await _context.Consumers.FindAsync(id);

            if (consumer == null || consumer.Deleted)
                return NotFound(new { message = "Consumer not found." });

            consumer.Deleted = true;
            _context.Consumers.Update(consumer);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Consumer deleted successfully." });
        }

        // Generate random password
        private static string GenerateRandomPassword(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789@#$!";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
