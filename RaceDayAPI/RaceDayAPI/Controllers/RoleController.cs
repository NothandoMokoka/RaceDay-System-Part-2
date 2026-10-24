using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;

namespace RaceDayAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly RaceDayContext _context;

        public RolesController(RaceDayContext context)
        {
            _context = context;
        }

        // GET: api/Roles
        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _context.Roles
                .Select(r => new
                {
                    r.RoleID,
                    r.RoleName,
                    r.Description
                })
                .ToListAsync();

            return Ok(roles);
        }
    }
}