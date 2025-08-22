using GuestRegistration.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GuestRegistration.Api.Controllers;

public class EventsController : ApiBaseController
{
    private readonly EventService _eventService;

    public EventsController(EventService eventService)
    {
        _eventService = eventService;
    }

    [HttpGet(Name = "GetEvents")]
    public async Task<IActionResult> GetEvents([FromQuery] string view = "future")
    {
        bool showFutureEvents = !string.Equals(view, "past", StringComparison.OrdinalIgnoreCase);

        var events = await _eventService.GetEventsAsync(showFutureEvents);
        
        return Ok(events);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEvent(long id)
    {
        var futureEvent = (await _eventService.GetEventsAsync(true)).Any(e => e.Id == id);
        if (!futureEvent)
        {
            return NotFound();
        }

        await _eventService.DeleteEventAsync(id);
        return NoContent();
    }
}