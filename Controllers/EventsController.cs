using EventManagementApi.DTOs.Events;
using EventManagementApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly IEventService _eventService;

    public EventsController(IEventService eventService)
    {
        _eventService = eventService;
    }

    [HttpGet]
    public async Task<ActionResult<List<EventResponseDto>>> GetAll()
    {
        return Ok(await _eventService.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EventResponseDto>> GetById(int id)
    {
        var result = await _eventService.GetByIdAsync(id);

        if (result is null)
            return NotFound(new { message = "Event not found." });

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<EventResponseDto>> Create(
        [FromBody] CreateEventDto dto)
    {
        try
        {
            var result = await _eventService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.EventId },
                result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<EventResponseDto>> Update(
        int id, [FromBody] UpdateEventDto dto)
    {
        try
        {
            var result = await _eventService.UpdateAsync(id, dto);

            if (result is null)
                return NotFound(new { message = "Event not found." });

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            bool deleted = await _eventService.DeleteAsync(id);

            if (!deleted)
                return NotFound(new { message = "Event not found." });

            return NoContent();
        }
        catch (DbUpdateException)
        {
            return Conflict(new
            {
                message = "The event cannot be deleted because related records exist."
            });
        }
    }
}
