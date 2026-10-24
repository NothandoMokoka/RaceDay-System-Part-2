using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.DTOs;
using RaceDayAPI.Models;

namespace RaceDayAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly RaceDayContext _context;

        public EventsController(RaceDayContext context)
        {
            _context = context;
        }

        private bool IsOrganizer()
        {
            return HttpContext.Session.GetString("RoleName") == "Organizer";
        }

        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            var events = await _context.Events
                .Select(e => new
                {
                    e.EventID,
                    e.OrganizerID,
                    e.Name,
                    e.Description,
                    e.EventDate,
                    e.Location,
                    e.Distance,
                    e.EventType,
                    e.CreatedAt
                })
                .ToListAsync();

            return Ok(events);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetEvent(int id)
        {
            var eventItem = await _context.Events
                .Where(e => e.EventID == id)
                .Select(e => new
                {
                    e.EventID,
                    e.OrganizerID,
                    e.Name,
                    e.Description,
                    e.EventDate,
                    e.Location,
                    e.Distance,
                    e.EventType,
                    e.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (eventItem == null)
                return NotFound(new { message = "Event not found." });

            return Ok(eventItem);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent(EventRequest request)
        {
            if (!IsOrganizer())
                return StatusCode(403, "Only organizers can create events.");

            var organizerID = HttpContext.Session.GetInt32("UserID");

            if (organizerID == null)
                return Unauthorized();

            var eventItem = new RaceDayAPI.Models.Event
            {
                OrganizerID = organizerID.Value,
                Name = request.Name,
                Description = request.Description,
                EventDate = request.EventDate,
                Location = request.Location,
                Distance = request.Distance,
                EventType = request.EventType,
                CreatedAt = DateTime.Now
            };

            _context.Events.Add(eventItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetEvent),
                new { id = eventItem.EventID },
                eventItem);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateEvent(
            int id, EventRequest request)
        {
            if (!IsOrganizer())
                return StatusCode(403, "Only organizers can update events.");

            var organizerID = HttpContext.Session.GetInt32("UserID");

            if (organizerID == null)
                return Unauthorized();

            var eventItem = await _context.Events
                .FirstOrDefaultAsync(e => e.EventID == id);

            if (eventItem == null)
                return NotFound(new { message = "Event not found." });

            if (eventItem.OrganizerID != organizerID.Value)
                return StatusCode(403, "You can only update your own events.");

            eventItem.Name = request.Name;
            eventItem.Description = request.Description;
            eventItem.EventDate = request.EventDate;
            eventItem.Location = request.Location;
            eventItem.Distance = request.Distance;
            eventItem.EventType = request.EventType;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Event updated successfully." });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            if (!IsOrganizer())
                return StatusCode(403, "Only organizers can delete events.");

            var organizerID = HttpContext.Session.GetInt32("UserID");

            if (organizerID == null)
                return Unauthorized();

            var eventItem = await _context.Events
                .FirstOrDefaultAsync(e => e.EventID == id);

            if (eventItem == null)
                return NotFound(new { message = "Event not found." });

            if (eventItem.OrganizerID != organizerID.Value)
                return StatusCode(403, "You can only delete your own events.");

            _context.Events.Remove(eventItem);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Event deleted successfully." });
        }
    }
}