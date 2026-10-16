using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventManagementApi.Data;
using EventManagementApi.Models.Entities;

namespace EventManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrganisersController : ControllerBase
{
    private readonly AppDbContext _context;

    public OrganisersController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/Organisers
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Organiser>>> GetOrganisers()
    {
        return await _context.Organisers
            .AsNoTracking()
            .ToListAsync();
    }

    // GET: api/Organisers/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Organiser>> GetOrganiser(int id)
    {
        var organiser = await _context.Organisers
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.OrganiserId == id);

        if (organiser == null)
            return NotFound();

        return Ok(organiser);
    }

    // POST: api/Organisers
    [HttpPost]
    public async Task<ActionResult<Organiser>> CreateOrganiser(
        Organiser organiser)
    {
        _context.Organisers.Add(organiser);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetOrganiser),
            new { id = organiser.OrganiserId },
            organiser);
    }

    // PUT: api/Organisers/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateOrganiser(
        int id,
        Organiser organiser)
    {
        if (id != organiser.OrganiserId)
            return BadRequest("Organiser ID does not match.");

        var existing = await _context.Organisers.FindAsync(id);

        if (existing == null)
            return NotFound();

        existing.OrganisationName = organiser.OrganisationName;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/Organisers/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteOrganiser(int id)
    {
        var organiser = await _context.Organisers.FindAsync(id);

        if (organiser == null)
            return NotFound();

        _context.Organisers.Remove(organiser);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}