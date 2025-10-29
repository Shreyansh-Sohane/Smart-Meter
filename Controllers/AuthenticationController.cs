using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SmartMeterBackend.Data.Context;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using SmartMeterBackend.Data.Entities;

namespace SmartMeterBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthenticationController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                 return BadRequest(ModelState);

            var exists = await _context.Users.AnyAsync(u => u.Phone == request.MobileNumber);
            if (exists)
                return BadRequest(new { message = "User already exists with this mobile number." });

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var newUser = new User
            {
                Username = request.Username,
                Displayname = request.Username,
                Phone = request.MobileNumber,
                Passwordhash = Encoding.UTF8.GetBytes(passwordHash),
                Email = request.Email,
                Isactive = true,
                Lastloginutc = null
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "User registered successfully.",
                userId = newUser.Userid,
                username = newUser.Username
            });
        }
        

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (request.Role.ToLower() == "user")
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Phone == request.MobileNumber);
                if (user == null)
                    return Unauthorized(new { message = "Invalid mobile number or password." });

                if (user.Isactive == false) return BadRequest("User is no longer active");

                var storedHash = Encoding.UTF8.GetString(user.Passwordhash);
                bool verified = BCrypt.Net.BCrypt.Verify(request.Password, storedHash);

                if (!verified)
                    return Unauthorized(new { message = "Invalid mobile number or password." });

                user.Lastloginutc = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                var token = GenerateJwtToken(user.Username, user.Userid, user.Phone ?? "", "user");

                return Ok(new
                {
                    message = "Login successful.",
                    token,
                    username = user.Username,
                    userId = user.Userid
                });
            }
            else if (request.Role.ToLower() == "consumer")
            {
                var consumer = await _context.Consumers.FirstOrDefaultAsync(c => c.Phone == request.MobileNumber);
                if (consumer == null)
                    return Unauthorized(new { message = "Invalid mobile number." });





                var token = GenerateJwtToken(consumer.Name, consumer.Consumerid, consumer.Phone ?? "", "consumer");

                return Ok(new
                {
                    message = "Login successful.",
                    token,
                    name = consumer.Name,
                    consumerId = consumer.Consumerid
                });
            }

            return BadRequest(new { message = "Invalid role." });
        }
        
        private string GenerateJwtToken(string username, long id, string phone, string role)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                new Claim(ClaimTypes.Name, username),
                new Claim("mobile", phone),
                new Claim(ClaimTypes.Role, role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT Key not configured.")));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"] ?? _configuration["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    // ✅ DTOs
    public class RegisterRequest
    {
        [Required]
        public required string MobileNumber { get; set; }

        [Required, MinLength(6)]
        public required string Password { get; set; }

        [Required]
        public required string Username { get; set; }

        [Required]
        public required string Email { get; set; }
    }

    public class LoginRequest
    {
        [Required]
        public required string MobileNumber { get; set; }

        [Required]
        public required string Password { get; set; }

        [Required]
        public required string Role { get; set; }
    }
}
