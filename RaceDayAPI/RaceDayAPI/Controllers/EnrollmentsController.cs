using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.DTOs;
using RaceDayAPI.Models;

namespace RaceDayAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnrollmentsController : ControllerBase
    {
        private readonly RaceDayContext _context;

        public EnrollmentsController(RaceDayContext context)
        {
            _context = context;
        }

        private string? RoleName =>
            HttpContext.Session.GetString("RoleName");

        private int? UserID =>
            HttpContext.Session.GetInt32("UserID");

        [HttpGet]
        public async Task<IActionResult> GetEnrollments()
        {
            if (UserID == null)
                return Unauthorized();

            if (RoleName == "Organizer")
            {
                var allEnrollments = await _context.Enrollments
                    .Select(e => new
                    {
                        e.EnrollmentID,
                        e.ParticipantID,
                        e.EventID,
                        e.CategoryID,
                        e.EnrollmentDate,
                        e.Status
                    })
                    .ToListAsync();

                return Ok(allEnrollments);
            }

            if (RoleName != "Participant")
                return StatusCode(403, "You are not authorized to view enrollments.");

            var enrollments = await _context.Enrollments
                .Where(e => e.ParticipantID == UserID.Value)
                .Select(e => new
                {
                    e.EnrollmentID,
                    e.ParticipantID,
                    e.EventID,
                    e.CategoryID,
                    e.EnrollmentDate,
                    e.Status
                })
                .ToListAsync();

            return Ok(enrollments);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetEnrollment(int id)
        {
            if (UserID == null)
                return Unauthorized();

            var enrollment = await _context.Enrollments
                .Where(e => e.EnrollmentID == id)
                .Select(e => new
                {
                    e.EnrollmentID,
                    e.ParticipantID,
                    e.EventID,
                    e.CategoryID,
                    e.EnrollmentDate,
                    e.Status
                })
                .FirstOrDefaultAsync();

            if (enrollment == null)
                return NotFound();

            if (RoleName != "Organizer" &&
                (RoleName != "Participant" ||
                 enrollment.ParticipantID != UserID.Value))
                return StatusCode(403, "You cannot view this enrollment.");

            return Ok(enrollment);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEnrollment(
            EnrollmentRequest request)
        {
            if (UserID == null)
                return Unauthorized();

            if (RoleName != "Participant")
                return StatusCode(403, "Only participants can enroll in events.");

            var eventExists = await _context.Events
                .AnyAsync(e => e.EventID == request.EventID);

            if (!eventExists)
                return BadRequest(new { message = "Event does not exist." });

            var category = await _context.Categories
                .FirstOrDefaultAsync(c =>
                    c.CategoryID == request.CategoryID &&
                    c.EventID == request.EventID);

            if (category == null)
                return BadRequest(new
                {
                    message = "Category does not belong to this event."
                });

            var alreadyEnrolled = await _context.Enrollments
                .AnyAsync(e =>
                    e.ParticipantID == UserID.Value &&
                    e.EventID == request.EventID);

            if (alreadyEnrolled)
                return Conflict(new
                {
                    message = "You are already enrolled in this event."
                });

            var enrollment = new Enrollment
            {
                ParticipantID = UserID.Value,
                EventID = request.EventID,
                CategoryID = request.CategoryID,
                EnrollmentDate = DateTime.Now,
                Status = "CONFIRMED"
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Enrollment successful.",
                enrollment.EnrollmentID,
                enrollment.ParticipantID,
                enrollment.EventID,
                enrollment.CategoryID,
                enrollment.EnrollmentDate,
                enrollment.Status
            });
        }

        [HttpPut("{id:int}/cancel")]
        public async Task<IActionResult> CancelEnrollment(int id)
        {
            if (UserID == null)
                return Unauthorized();

            if (RoleName != "Participant")
                return StatusCode(403, "Only participants can cancel enrollments.");

            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.EnrollmentID == id);

            if (enrollment == null)
                return NotFound();

            if (enrollment.ParticipantID != UserID.Value)
                return StatusCode(403, "You can only cancel your own enrollment.");

            if (enrollment.Status == "CANCELLED")
                return Conflict(new
                {
                    message = "Enrollment is already cancelled."
                });

            enrollment.Status = "CANCELLED";
            await _context.SaveChangesAsync();

            return Ok(new { message = "Enrollment cancelled successfully." });
        }
    }
}