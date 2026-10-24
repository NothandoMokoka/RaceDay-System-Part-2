using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.DTOs;
using RaceDayAPI.Models;

namespace RaceDayAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly RaceDayContext _context;

        public CategoriesController(RaceDayContext context)
        {
            _context = context;
        }

        private bool IsOrganizer()
        {
            return HttpContext.Session.GetString("RoleName") == "Organizer";
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories
                .Select(c => new
                {
                    c.CategoryID,
                    c.EventID,
                    c.CategoryName,
                    c.MinimumAge,
                    c.MaximumAge,
                    c.Distance
                })
                .ToListAsync();

            return Ok(categories);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _context.Categories
                .Where(c => c.CategoryID == id)
                .Select(c => new
                {
                    c.CategoryID,
                    c.EventID,
                    c.CategoryName,
                    c.MinimumAge,
                    c.MaximumAge,
                    c.Distance
                })
                .FirstOrDefaultAsync();

            if (category == null)
                return NotFound(new { message = "Category not found." });

            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CategoryRequest request)
        {
            if (!IsOrganizer())
                return StatusCode(403, "Only organizers can create categories.");

            var eventExists = await _context.Events
                .AnyAsync(e => e.EventID == request.EventID);

            if (!eventExists)
                return BadRequest(new { message = "Event does not exist." });

            if (request.MinimumAge > request.MaximumAge)
                return BadRequest(new { message = "Minimum age cannot exceed maximum age." });

            var duplicate = await _context.Categories.AnyAsync(c =>
                c.EventID == request.EventID &&
                c.CategoryName == request.CategoryName);

            if (duplicate)
                return Conflict(new { message = "This category already exists for the event." });

            var category = new Category
            {
                EventID = request.EventID,
                CategoryName = request.CategoryName,
                MinimumAge = request.MinimumAge,
                MaximumAge = request.MaximumAge,
                Distance = request.Distance
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetCategory),
                new { id = category.CategoryID },
                category);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCategory(
            int id, CategoryRequest request)
        {
            if (!IsOrganizer())
                return StatusCode(403, "Only organizers can update categories.");

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryID == id);

            if (category == null)
                return NotFound(new { message = "Category not found." });

            var eventExists = await _context.Events
                .AnyAsync(e => e.EventID == request.EventID);

            if (!eventExists)
                return BadRequest(new { message = "Event does not exist." });

            if (request.MinimumAge > request.MaximumAge)
                return BadRequest(new { message = "Minimum age cannot exceed maximum age." });

            var duplicate = await _context.Categories.AnyAsync(c =>
                c.CategoryID != id &&
                c.EventID == request.EventID &&
                c.CategoryName == request.CategoryName);

            if (duplicate)
                return Conflict(new { message = "This category already exists for the event." });

            category.EventID = request.EventID;
            category.CategoryName = request.CategoryName;
            category.MinimumAge = request.MinimumAge;
            category.MaximumAge = request.MaximumAge;
            category.Distance = request.Distance;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Category updated successfully." });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if (!IsOrganizer())
                return StatusCode(403, "Only organizers can delete categories.");

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryID == id);

            if (category == null)
                return NotFound(new { message = "Category not found." });

            var hasEnrollments = await _context.Enrollments
                .AnyAsync(e => e.CategoryID == id);

            if (hasEnrollments)
                return Conflict(new
                {
                    message = "Cannot delete a category that has enrollments."
                });

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Category deleted successfully." });
        }
    }
}