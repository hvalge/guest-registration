using GuestRegistration.Application.DTOs;
using GuestRegistration.Core.Entities;
using GuestRegistration.Core.Interfaces;

namespace GuestRegistration.Application.Services;

public class ParticipantService
{
    private readonly IParticipantRepository _participantRepository;
    private readonly IEventRepository _eventRepository;

    public ParticipantService(IParticipantRepository participantRepository, IEventRepository eventRepository)
    {
        _participantRepository = participantRepository;
        _eventRepository = eventRepository;
    }

    public async Task AddParticipantToEventAsync(long eventId, CreateParticipantDto dto)
    {
        var eventEntity = await _eventRepository.GetByIdAsync(eventId);
        if (eventEntity == null)
        {
            throw new Exception("Event not found");
        }

        Participant participant;
        if (dto.Type == Core.Enums.ParticipantType.NaturalPerson)
        {
            if (!IsValidEstonianIdCode(dto.IdCode!))
            {
                throw new ArgumentException("Invalid Estonian ID code.");
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
    }

    public async Task<List<PaymentMethod>> GetPaymentMethodsAsync()
    {
        return await _participantRepository.GetPaymentMethodsAsync();
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