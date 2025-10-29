using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartMeterBackend.Data.Context;
using SmartMeterBackend.Models.DTOs;
using System.Security.Claims;
using System.Text;

namespace SmartMeterBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private ApplicationDbContext _context;

        public UserController(ILogger<UserController> logger , ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Authorize(Roles = "user")]
        [HttpPost("upload")]
        public async Task<IActionResult> UploadToCloudinary(IFormFile file, [FromServices] Cloudinary cloudinary)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { error = "No file uploaded or file is empty." });

            try
            {
                await using var stream = file.OpenReadStream();

                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = "my_app_uploads"
                };

                var uploadResult = await cloudinary.UploadAsync(uploadParams);

                if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    _logger.LogWarning("Cloudinary upload failed: {Error}", uploadResult.Error?.Message);
                    return StatusCode(StatusCodes.Status502BadGateway, new
                    {
                        error = "Failed to upload image to Cloudinary.",
                        cloudinaryError = uploadResult.Error?.Message
                    });
                }
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!long.TryParse(userIdString, out var userId))
                    return BadRequest(new { error = "Invalid user ID." });

                var user = await _context.Users.FindAsync(userId);

                if (user == null)
                    return NotFound(new { error = "User not found." });

                user.ImageUrl = uploadResult.SecureUrl?.ToString();
                await _context.SaveChangesAsync();


                return Ok(new
                {
                    Url = uploadResult.SecureUrl?.ToString(),
                    PublicId = uploadResult.PublicId
                });
            }


            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while uploading file.");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    error = "An unexpected error occurred during upload.",
                    details = ex.Message
                });
            }
        }

        [Authorize(Roles ="user")]
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Phone == request.mobileNumber);
            if (user == null)
                return Unauthorized(new { message = "Invalid mobile number or password." });

            var storedHash = Encoding.UTF8.GetString(user.Passwordhash);
            bool verified = BCrypt.Net.BCrypt.Verify(request.oldPassword, storedHash);

            if (!verified)
                return Unauthorized(new { message = "Invalid old password." });

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.newPassword);

            user.Passwordhash = Encoding.UTF8.GetBytes(passwordHash);

            return Ok(new
            {
                message = "Password change successful.",
               
                username = user.Username,
                userId = user.Userid
            });
        }

        //[Authorize(Roles = "user")]
        [HttpPost("GetUser/{mobileNumber}")]
        public async Task<IActionResult> GetUser([FromRoute] string mobileNumber)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Phone == mobileNumber);
            if (user == null)
                return Unauthorized(new { message = "Invalid mobile number or password." });
  
            return Ok(new{user});
        }
    }
}
