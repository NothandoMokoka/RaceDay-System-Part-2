using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.DTOs;

namespace RaceDayAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly RaceDayContext _context;

        public UsersController(RaceDayContext context)
        {
            _context = context;
        }

        private int? UserID =>
            HttpContext.Session.GetInt32("UserID");

        private string? RoleName =>
            HttpContext.Session.GetString("RoleName");

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            if (UserID == null)
                return Unauthorized();

            if (RoleName != "Organizer")
                return StatusCode(403, "Only organizers can view all users.");

            var users = await _context.Users
                .Select(u => new UserResponseDto
                {
                    UserID = u.UserID,
                    RoleID = u.RoleID,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    CreatedDate = u.CreatedDate
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUser(int id)
        {
            if (UserID == null)
                return Unauthorized();

            if (RoleName != "Organizer" && UserID != id)
                return StatusCode(403, "You can only view your own profile.");

            var user = await _context.Users
                .Where(u => u.UserID == id)
                .Select(u => new UserResponseDto
                {
                    UserID = u.UserID,
                    RoleID = u.RoleID,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    CreatedDate = u.CreatedDate
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateUser(
            int id, UserResponseDto request)
        {
            if (UserID == null)
                return Unauthorized();

            if (UserID != id)
                return StatusCode(403, "You can only update your own profile.");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserID == id);

            if (user == null)
                return NotFound();

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.PhoneNumber = request.PhoneNumber;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Profile updated successfully." });
        }
    }
}