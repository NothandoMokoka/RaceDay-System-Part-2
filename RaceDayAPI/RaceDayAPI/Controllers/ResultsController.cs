using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.DTOs;
using RaceDayAPI.Models;

namespace RaceDayAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResultsController : ControllerBase
    {
        private readonly RaceDayContext _context;

        public ResultsController(RaceDayContext context)
        {
            _context = context;
        }

        private int? UserID =>
            HttpContext.Session.GetInt32("UserID");

        private string? RoleName =>
            HttpContext.Session.GetString("RoleName");

        [HttpGet]
        public async Task<IActionResult> GetResults()
        {
            if (UserID == null)
                return Unauthorized();

            if (RoleName == "Organizer")
            {
                var allResults = await _context.Results
                    .Select(r => new
                    {
                        r.ResultID,
                        r.EnrollmentID,
                        r.FinishPosition,
                        r.FinishTime,
                        r.RecordedAt
                    })
                    .ToListAsync();

                return Ok(allResults);
            }

            if (RoleName != "Participant")
                return StatusCode(403, "You are not authorized to view results.");

            var results = await _context.Results
                .Where(r => r.Enrollment != null &&
                            r.Enrollment.ParticipantID == UserID.Value)
                .Select(r => new
                {
                    r.ResultID,
                    r.EnrollmentID,
                    r.FinishPosition,
                    r.FinishTime,
                    r.RecordedAt
                })
                .ToListAsync();

            return Ok(results);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetResult(int id)
        {
            if (UserID == null)
                return Unauthorized();

            var result = await _context.Results
                .Include(r => r.Enrollment)
                .FirstOrDefaultAsync(r => r.ResultID == id);

            if (result == null)
                return NotFound();

            if (RoleName != "Organizer" &&
                (RoleName != "Participant" ||
                 result.Enrollment?.ParticipantID != UserID.Value))
                return StatusCode(403, "You are not authorized to view this result.");

            return Ok(new
            {
                result.ResultID,
                result.EnrollmentID,
                result.FinishPosition,
                result.FinishTime,
                result.RecordedAt
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateResult(ResultRequest request)
        {
            if (UserID == null)
                return Unauthorized();

            if (RoleName != "Organizer")
                return StatusCode(403, "Only organizers can record results.");

            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.EnrollmentID == request.EnrollmentID);

            if (enrollment == null)
                return BadRequest(new
                {
                    message = "Enrollment does not exist."
                });

            if (enrollment.Status != "CONFIRMED")
                return BadRequest(new
                {
                    message = "A result can only be recorded for a confirmed enrollment."
                });

            var existingResult = await _context.Results
                .AnyAsync(r => r.EnrollmentID == request.EnrollmentID);

            if (existingResult)
                return Conflict(new
                {
                    message = "A result already exists for this enrollment."
                });

            var positionTaken = await _context.Results
                .AnyAsync(r => r.FinishPosition == request.FinishPosition);

            if (positionTaken)
                return Conflict(new
                {
                    message = "This finish position is already in use."
                });

            var result = new Result
            {
                EnrollmentID = request.EnrollmentID,
                FinishPosition = request.FinishPosition,
                FinishTime = request.FinishTime,
                RecordedAt = DateTime.Now
            };

            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Result recorded successfully.",
                result.ResultID,
                result.EnrollmentID,
                result.FinishPosition,
                result.FinishTime,
                result.RecordedAt
            });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateResult(
            int id, ResultRequest request)
        {
            if (UserID == null)
                return Unauthorized();

            if (RoleName != "Organizer")
                return StatusCode(403, "Only organizers can update results.");

            var result = await _context.Results
                .FirstOrDefaultAsync(r => r.ResultID == id);

            if (result == null)
                return NotFound();

            var enrollmentExists = await _context.Enrollments
                .AnyAsync(e => e.EnrollmentID == request.EnrollmentID);

            if (!enrollmentExists)
                return BadRequest(new
                {
                    message = "Enrollment does not exist."
                });

            var duplicateEnrollment = await _context.Results.AnyAsync(r =>
                r.ResultID != id &&
                r.EnrollmentID == request.EnrollmentID);

            if (duplicateEnrollment)
                return Conflict(new
                {
                    message = "A result already exists for this enrollment."
                });

            var positionTaken = await _context.Results.AnyAsync(r =>
                r.ResultID != id &&
                r.FinishPosition == request.FinishPosition);

            if (positionTaken)
                return Conflict(new
                {
                    message = "This finish position is already in use."
                });

            result.EnrollmentID = request.EnrollmentID;
            result.FinishPosition = request.FinishPosition;
            result.FinishTime = request.FinishTime;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Result updated successfully." });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteResult(int id)
        {
            if (UserID == null)
                return Unauthorized();

            if (RoleName != "Organizer")
                return StatusCode(403, "Only organizers can delete results.");

            var result = await _context.Results
                .FirstOrDefaultAsync(r => r.ResultID == id);

            if (result == null)
                return NotFound();

            _context.Results.Remove(result);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Result deleted successfully." });
        }
    }
}