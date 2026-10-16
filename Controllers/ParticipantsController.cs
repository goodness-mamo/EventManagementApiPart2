
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventManagementApi.Data;
using EventManagementApi.Models.Entities;

namespace EventManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ParticipantsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ParticipantsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/Participants
    [HttpGet]
    public async Task<IActionResult> GetParticipants()
    {
        var participants = await _context.Participants
            .AsNoTracking()
            .Select(p => new
            {
                p.ParticipantId,
                p.UserId,
                FirstName = p.User.FirstName,
                LastName = p.User.LastName,
                Email = p.User.Email,
                p.DateOfBirth,
                p.EmergencyContactName,
                p.EmergencyContactNumber
            })
            .ToListAsync();

        return Ok(participants);
    }

    // GET: api/Participants/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetParticipant(int id)
    {
        var participant = await _context.Participants
            .AsNoTracking()
            .Where(p => p.ParticipantId == id)
            .Select(p => new
            {
                p.ParticipantId,
                p.UserId,
                FirstName = p.User.FirstName,
                LastName = p.User.LastName,
                Email = p.User.Email,
                p.DateOfBirth,
                p.EmergencyContactName,
                p.EmergencyContactNumber
            })
            .FirstOrDefaultAsync();

        if (participant == null)
        {
            return NotFound();
        }

        return Ok(participant);
    }

    // POST: api/Participants
    [HttpPost]
    public async Task<IActionResult> CreateParticipant(
        [FromBody] Participant participant)
    {
        // Check that the referenced user exists.
        var userExists = await _context.Users
            .AnyAsync(u => u.UserId == participant.UserId);

        if (!userExists)
        {
            return BadRequest(
                "The specified UserId does not exist.");
        }

        // Prevent duplicate participant records for the same user.
        var alreadyRegistered = await _context.Participants
            .AnyAsync(p => p.UserId == participant.UserId);

        if (alreadyRegistered)
        {
            return Conflict(
                "A participant record already exists for this user.");
        }

        // Allow the database to generate the ParticipantID
        // if the column is configured as an identity column.
        participant.ParticipantId = 0;

        _context.Participants.Add(participant);

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetParticipant),
            new { id = participant.ParticipantId },
            new
            {
                participant.ParticipantId,
                participant.UserId,
                participant.DateOfBirth,
                participant.EmergencyContactName,
                participant.EmergencyContactNumber
            });
    }

    // PUT: api/Participants/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateParticipant(
        int id,
        [FromBody] Participant participant)
    {
        if (id != participant.ParticipantId)
        {
            return BadRequest(
                "Participant ID does not match.");
        }

        var existing = await _context.Participants
            .FirstOrDefaultAsync(p => p.ParticipantId == id);

        if (existing == null)
        {
            return NotFound();
        }

        // Update participant-specific fields only.
        existing.DateOfBirth = participant.DateOfBirth;
        existing.EmergencyContactName =
            participant.EmergencyContactName;
        existing.EmergencyContactNumber =
            participant.EmergencyContactNumber;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/Participants/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteParticipant(int id)
    {
        var participant = await _context.Participants
            .FindAsync(id);

        if (participant == null)
        {
            return NotFound();
        }

        _context.Participants.Remove(participant);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
