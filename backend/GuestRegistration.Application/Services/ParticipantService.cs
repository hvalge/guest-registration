using GuestRegistration.Application.DTOs;
using GuestRegistration.Application.Exceptions;
using GuestRegistration.Core.Entities;
using GuestRegistration.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace GuestRegistration.Application.Services;

public class ParticipantService
{
    private readonly IParticipantRepository _participantRepository;
    private readonly IEventRepository _eventRepository;
    private readonly ILogger<ParticipantService> _logger;

    public ParticipantService(IParticipantRepository participantRepository, IEventRepository eventRepository, ILogger<ParticipantService> logger)
    {
        _participantRepository = participantRepository;
        _eventRepository = eventRepository;
        _logger = logger;
    }

    public async Task<ParticipantDetailDto> AddParticipantToEventAsync(long eventId, CreateParticipantDto dto)
    {
        _logger.LogInformation("Adding participant of type {ParticipantType} to event {EventId}", dto.Type, eventId);
        var eventEntity = await _eventRepository.GetByIdAsync(eventId);
        if (eventEntity == null)
        {
            _logger.LogWarning("Event not found with ID: {EventId}", eventId);
            throw new NotFoundException($"Event with ID {eventId} not found.");
        }
        
        Participant participant;
        if (dto.Type == Core.Enums.ParticipantType.NaturalPerson)
        {
            if (!IsValidEstonianIdCode(dto.IdCode!))
            {
                _logger.LogWarning("Invalid Estonian ID code provided: {IdCode}", dto.IdCode);
                throw new ValidationException("Invalid Estonian ID code.");
            }

            participant = new NaturalPerson
            {
                FirstName = dto.FirstName!,
                LastName = dto.LastName!,
                IdCode = dto.IdCode!
            };
        }
        else
        {
            participant = new LegalPerson
            {
                CompanyName = dto.CompanyName!,
                RegisterCode = dto.RegisterCode!
            };
        }

        var eventParticipant = new EventParticipant
        {
            Event = eventEntity,
            Participant = participant,
            PaymentMethodId = dto.PaymentMethodId,
            NumberOfAttendees = dto.Type == Core.Enums.ParticipantType.LegalPerson ? dto.NumberOfAttendees : null,
            AdditionalInformation = dto.Type == Core.Enums.ParticipantType.NaturalPerson
                ? dto.AdditionalInformationNatural
                : dto.AdditionalInformationLegal
        };

        eventEntity.EventParticipants.Add(eventParticipant);
        await _participantRepository.AddAsync(participant);

        _logger.LogInformation("Successfully added participant {ParticipantId} to event {EventId}", participant.Id, eventId);
        
        return (await GetParticipantDetailsAsync(eventId, participant.Id))!;
    }
    
    public async Task<ParticipantDetailDto?> GetParticipantDetailsAsync(long eventId, long participantId)
    {
        _logger.LogInformation("Fetching details for participant {ParticipantId} from event {EventId}", participantId, eventId);
        var eventParticipant = await _participantRepository.GetEventParticipantAsync(eventId, participantId);
        if (eventParticipant == null)
        {
            _logger.LogWarning("Participant {ParticipantId} not found in event {EventId}", participantId, eventId);
            return null;
        }

        var participant = eventParticipant.Participant;
        var dto = new ParticipantDetailDto
        {
            Id = participant.Id,
            PaymentMethodId = eventParticipant.PaymentMethodId,
            AdditionalInformation = eventParticipant.AdditionalInformation
        };

        if (participant is NaturalPerson np)
        {
            dto.Type = Core.Enums.ParticipantType.NaturalPerson;
            dto.FirstName = np.FirstName;
            dto.LastName = np.LastName;
            dto.IdCode = np.IdCode;
        }
        else if (participant is LegalPerson lp)
        {
            dto.Type = Core.Enums.ParticipantType.LegalPerson;
            dto.CompanyName = lp.CompanyName;
            dto.RegisterCode = lp.RegisterCode;
            dto.NumberOfAttendees = eventParticipant.NumberOfAttendees;
        }

        return dto;
    }
    
    public async Task RemoveParticipantFromEventAsync(long eventId, long participantId)
    {
        _logger.LogInformation("Removing participant {ParticipantId} from event {EventId}", participantId, eventId);
        
        var success = await _eventRepository.RemoveParticipantAsync(eventId, participantId);
        if (!success)
        {
            throw new NotFoundException($"Participant {participantId} not found in event {eventId}.");
        }
        
        _logger.LogInformation("Participant {ParticipantId} removed from event {EventId}", participantId, eventId);
    }

    public async Task UpdateParticipantDetailsAsync(long eventId, long participantId, UpdateParticipantDto dto)
    {
        _logger.LogInformation("Updating details for participant {ParticipantId} in event {EventId}", participantId, eventId);
        var eventParticipant = await _participantRepository.GetEventParticipantAsync(eventId, participantId);
        if (eventParticipant == null)
        {
            _logger.LogWarning("Participant {ParticipantId} not found in event {EventId} for update", participantId, eventId);
            throw new Exception("Participant not found in this event.");
        }

        var participant = eventParticipant.Participant;

        if (dto.Type == Core.Enums.ParticipantType.NaturalPerson && participant is NaturalPerson np)
        {
            np.FirstName = dto.FirstName!;
            np.LastName = dto.LastName!;
            np.IdCode = dto.IdCode!;
        }
        else if (dto.Type == Core.Enums.ParticipantType.LegalPerson && participant is LegalPerson lp)
        {
            lp.CompanyName = dto.CompanyName!;
            lp.RegisterCode = dto.RegisterCode!;
            eventParticipant.NumberOfAttendees = dto.NumberOfAttendees;
        }

        eventParticipant.PaymentMethodId = dto.PaymentMethodId;
        eventParticipant.AdditionalInformation = dto.AdditionalInformation;

        await _participantRepository.UpdateAsync(eventParticipant);
        _logger.LogInformation("Successfully updated participant {ParticipantId} in event {EventId}", participantId, eventId);
    }
    

    private bool IsValidEstonianIdCode(string idCode)
    {
        if (string.IsNullOrEmpty(idCode) || idCode.Length != 11 || !idCode.All(char.IsDigit))
            return false;

        int[] weights1 = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 1 };
        int[] weights2 = { 3, 4, 5, 6, 7, 8, 9, 1, 2, 3 };

        int sum = 0;
        for (int i = 0; i < 10; i++)
        {
            sum += (int)char.GetNumericValue(idCode[i]) * weights1[i];
        }

        int remainder = sum % 11;
        if (remainder == 10)
        {
            sum = 0;
            for (int i = 0; i < 10; i++)
            {
                sum += (int)char.GetNumericValue(idCode[i]) * weights2[i];
            }

            remainder = sum % 11;
            if (remainder == 10)
            {
                remainder = 0;
            }
        }

        return remainder == (int)char.GetNumericValue(idCode[10]);
    }
}