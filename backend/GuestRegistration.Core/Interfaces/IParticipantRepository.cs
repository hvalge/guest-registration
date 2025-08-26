using GuestRegistration.Core.Entities;

namespace GuestRegistration.Core.Interfaces;

public interface IParticipantRepository
{
    Task AddAsync(Participant participant);
    Task<EventParticipant?> GetEventParticipantAsync(long eventId, long participantId);
    Task UpdateAsync(EventParticipant eventParticipant);
}