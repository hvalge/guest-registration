using GuestRegistration.Application.DTOs;
using GuestRegistration.Core.Interfaces;

namespace GuestRegistration.Application.Services;

public class EventService
{
    private readonly IEventRepository _eventRepository;

    public EventService(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task<IEnumerable<EventSummaryDto>> GetEventsAsync(bool showFutureEvents)
    {
        var events = await _eventRepository.GetEventsAsync(showFutureEvents);
        
        var eventDtos = events.Select(e => new EventSummaryDto
        {
            Id = e.Id,
            Name = e.Name,
            StartTime = e.StartTime,
            Location = e.Location,
            ParticipantCount = e.EventParticipants.Count
        });

        return eventDtos;
    }
    
    public async Task DeleteEventAsync(long id)
    {
        await _eventRepository.DeleteEventAsync(id);
    }
}