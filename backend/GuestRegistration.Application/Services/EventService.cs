using GuestRegistration.Application.DTOs;
using GuestRegistration.Core.Entities;
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
    
    public async Task<Event?> GetEventByIdAsync(long id)
    {
        return await _eventRepository.GetByIdAsync(id);
    }

    public async Task DeleteEventAsync(long id)
    {
        await _eventRepository.DeleteEventAsync(id);
    }

    public async Task CreateEventAsync(CreateEventDto createEventDto)
    {
        if (createEventDto.StartTime < DateTime.UtcNow)
        {
            throw new ArgumentException("Event start time must be in the future.");
        }
        
        var newEvent = new Event
        {
            Name = createEventDto.Name,
            StartTime = createEventDto.StartTime,
            Location = createEventDto.Location,
            AdditionalInformation = createEventDto.AdditionalInformation
        };

        await _eventRepository.AddAsync(newEvent);
    }
}