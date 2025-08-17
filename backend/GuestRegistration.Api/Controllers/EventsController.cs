using GuestRegistration.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GuestRegistration.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class EventsController : ControllerBase
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
}