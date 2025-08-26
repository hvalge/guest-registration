using GuestRegistration.Application.DTOs;
using GuestRegistration.Application.Exceptions;
using GuestRegistration.Core.Entities;
using GuestRegistration.Core.Interfaces;
using Microsoft.Extensions.Logging;
using ValidationException = GuestRegistration.Application.Exceptions.ValidationException;

namespace GuestRegistration.Application.Services;

public class EventService
{
    private readonly IEventRepository _eventRepository;
    private readonly ILogger<EventService> _logger;

    public EventService(IEventRepository eventRepository, ILogger<EventService> logger)
    {
        _eventRepository = eventRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<EventSummaryDto>> GetEventsAsync(bool showFutureEvents)
    {
        _logger.LogInformation("Fetching {EventType} events", showFutureEvents ? "future" : "past");
        var events = await _eventRepository.GetEventsAsync(showFutureEvents);
        
        return events.Select(e => new EventSummaryDto
        {
            Id = e.Id,
            Name = e.Name,
            StartTime = e.StartTime,
            Location = e.Location,
            ParticipantCount = e.EventParticipants.Count
        });
    }

    public async Task<EventDetailsDto?> GetEventDetailsAsync(long id)
    {
        _logger.LogInformation("Fetching event details for event ID: {EventId}", id);
        var eventEntity = await _eventRepository.GetByIdWithParticipantsAsync(id);
        if (eventEntity == null)
        {
            _logger.LogWarning("Event with ID: {EventId} not found", id);
            return null;
        }
        
        return new EventDetailsDto
        {
            Id = eventEntity.Id,
            Name = eventEntity.Name,
            Participants = eventEntity.EventParticipants.Select(ep =>
            {
                var participant = ep.Participant;
                return new ParticipantDto
                {
                    Id = participant.Id,
                    Name = participant is NaturalPerson np ? $"{np.FirstName} {np.LastName}" : ((LegalPerson)participant).CompanyName,
                    Code = participant is NaturalPerson np2 ? np2.IdCode : ((LegalPerson)participant).RegisterCode
                };
            }).ToList()
        };
    }

    public async Task<EventDetailsDto> CreateEventAsync(CreateEventDto createEventDto)
    {
        _logger.LogInformation("Creating a new event with name: {EventName}", createEventDto.Name);
        if (createEventDto.StartTime < DateTime.UtcNow)
        {
            _logger.LogWarning("Attempted to create an event in the past: {EventStartTime}", createEventDto.StartTime);
            throw new ValidationException("Event start time must be in the future.");
        }

        var newEvent = new Event
        {
            Name = createEventDto.Name,
            StartTime = createEventDto.StartTime,
            Location = createEventDto.Location,
            AdditionalInformation = createEventDto.AdditionalInformation
        };

        await _eventRepository.AddAsync(newEvent);
        _logger.LogInformation("Successfully created event {EventName} with ID {EventId}", newEvent.Name, newEvent.Id);
        
        return (await GetEventDetailsAsync(newEvent.Id))!;
    }
    
    public async Task DeleteEventAsync(long id)
    {
        _logger.LogInformation("Deleting event with ID: {EventId}", id);
        var eventToDelete = await _eventRepository.GetByIdAsync(id);

        if (eventToDelete == null)
        {
            throw new NotFoundException($"Event with ID {id} not found.");
        }

        if (eventToDelete.StartTime < DateTime.UtcNow)
        {
            throw new BusinessRuleValidationException("Cannot delete an event that has already started.");
        }
        
        await _eventRepository.DeleteEventAsync(id);
        _logger.LogInformation("Event with ID: {EventId} deleted", id);
    }
}
