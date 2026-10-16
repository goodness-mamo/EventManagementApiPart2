using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventManagementApi.Data;
using EventManagementApi.Models.Entities;

namespace EventManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnrolmentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public EnrolmentsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/Enrolments
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Enrolments>>> GetEnrolments()
    {
        return await _context.Enrolments
            .AsNoTracking()
            .ToListAsync();
    }

    // GET: api/Enrolments/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Enrolments>> GetEnrolment(int id)
    {
        var enrolment = await _context.Enrolments
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.EnrolmentId == id);

        if (enrolment == null)
            return NotFound();

        return Ok(enrolment);
    }

    // POST: api/Enrolments
    [HttpPost]
    public async Task<ActionResult<Enrolments>> CreateEnrolment(
        Enrolments enrolment)
    {
        var eventCategoryExists = await _context.EventCategories
            .AnyAsync(ec =>
                ec.EventId == enrolment.EventId &&
                ec.CategoryId == enrolment.CategoryId);

        if (!eventCategoryExists)
            return BadRequest(
                "This category is not associated with the selected event.");

        var participantExists = await _context.Participants
            .AnyAsync(p => p.ParticipantId == enrolment.ParticipantId);

        if (!participantExists)
            return BadRequest("Participant does not exist.");

        var duplicateExists = await _context.Enrolments
            .AnyAsync(e =>
                e.EventId == enrolment.EventId &&
                e.CategoryId == enrolment.CategoryId &&
                e.ParticipantId == enrolment.ParticipantId);

        if (duplicateExists)
            return Conflict(
                "This participant is already enrolled in this event category.");

        enrolment.EnrolledAt = DateTime.UtcNow;

        _context.Enrolments.Add(enrolment);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetEnrolment),
            new { id = enrolment.EnrolmentId },
            enrolment);
    }

    // PUT: api/Enrolments/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateEnrolment(
        int id,
        Enrolments enrolment)
    {
        if (id != enrolment.EnrolmentId)
            return BadRequest("Enrolment ID does not match.");

        var existing = await _context.Enrolments.FindAsync(id);

        if (existing == null)
            return NotFound();

        var eventCategoryExists = await _context.EventCategories
            .AnyAsync(ec =>
                ec.EventId == enrolment.EventId &&
                ec.CategoryId == enrolment.CategoryId);

        if (!eventCategoryExists)
            return BadRequest(
                "This category is not associated with the selected event.");

        var participantExists = await _context.Participants
            .AnyAsync(p => p.ParticipantId == enrolment.ParticipantId);

        if (!participantExists)
            return BadRequest("Participant does not exist.");

        var duplicateExists = await _context.Enrolments
            .AnyAsync(e =>
                e.EnrolmentId != id &&
                e.EventId == enrolment.EventId &&
                e.CategoryId == enrolment.CategoryId &&
                e.ParticipantId == enrolment.ParticipantId);

        if (duplicateExists)
            return Conflict(
                "This participant is already enrolled in this event category.");

        existing.EventId = enrolment.EventId;
        existing.CategoryId = enrolment.CategoryId;
        existing.ParticipantId = enrolment.ParticipantId;
        existing.Status = enrolment.Status;
        existing.RaceNumber = enrolment.RaceNumber;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/Enrolments/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteEnrolment(int id)
    {
        var enrolment = await _context.Enrolments.FindAsync(id);

        if (enrolment == null)
            return NotFound();

        _context.Enrolments.Remove(enrolment);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
