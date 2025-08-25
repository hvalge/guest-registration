using GuestRegistration.Application.DTOs;
using GuestRegistration.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GuestRegistration.Api.Controllers;

[ApiController]
[Route("api")]
public class EventsController : ControllerBase
{
    private readonly EventService _eventService;

    public EventsController(EventService eventService)
    {
        _eventService = eventService;
    }

    [HttpGet("events")]
    public async Task<IActionResult> GetEvents([FromQuery] string view = "future")
    {
        bool showFutureEvents = !string.Equals(view, "past", StringComparison.OrdinalIgnoreCase);

        var events = await _eventService.GetEventsAsync(showFutureEvents);
        
        return Ok(events);
    }
    
    [HttpGet("events/{id}")]
    public async Task<IActionResult> GetEventDetails(long id)
    {
        var eventDetails = await _eventService.GetEventDetailsAsync(id);
        if (eventDetails == null)
        {
            return NotFound();
        }
        return Ok(eventDetails);
    }
    
    [HttpDelete("events/{eventId}/participants/{participantId}")]
    public async Task<IActionResult> DeleteParticipant(long eventId, long participantId)
    {
        await _eventService.RemoveParticipantFromEventAsync(eventId, participantId);
        return NoContent();
    }
    
    [HttpDelete("events/{id}")]
    public async Task<IActionResult> DeleteEvent(long id)
    {
        var eventToDelete = await _eventService.GetEventByIdAsync(id);

        if (eventToDelete == null)
        {
            return NotFound();
        }

        if (eventToDelete.StartTime < DateTime.UtcNow)
        {
            return BadRequest("Past events cannot be deleted.");
        }

        await _eventService.DeleteEventAsync(id);
        return NoContent();
    }
    
    [HttpPost("events")]
    public async Task<IActionResult> CreateEvent([FromBody] CreateEventDto createEventDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _eventService.CreateEventAsync(createEventDto);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}