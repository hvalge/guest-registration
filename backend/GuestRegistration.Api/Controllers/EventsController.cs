using GuestRegistration.Application.DTOs;
using GuestRegistration.Application.Services;
using GuestRegistration.Core.Enums;
using Microsoft.AspNetCore.Mvc;

namespace GuestRegistration.Api.Controllers;

[ApiController]
[Route("api/events")]
public class EventsController : ControllerBase
{
    private readonly EventService _eventService;

    public EventsController(EventService eventService)
    {
        _eventService = eventService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EventSummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEvents([FromQuery] EventView view = EventView.Future)
    {
        bool showFutureEvents = (view == EventView.Future);

        var events = await _eventService.GetEventsAsync(showFutureEvents);
        
        return Ok(events);
    }
    
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(EventDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEventDetails(long id)
    {
        var eventDetails = await _eventService.GetEventDetailsAsync(id);
        if (eventDetails == null)
        {
            return NotFound($"Event with ID {id} not found.");
        }
        return Ok(eventDetails);
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(EventDetailsDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateEvent([FromBody] CreateEventDto createEventDto)
    {
        var newEvent = await _eventService.CreateEventAsync(createEventDto);
        
        return CreatedAtAction(nameof(GetEventDetails), new { id = newEvent.Id }, newEvent);
    }
    
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteEvent(long id)
    {
        await _eventService.DeleteEventAsync(id);
        return NoContent();
    }
}