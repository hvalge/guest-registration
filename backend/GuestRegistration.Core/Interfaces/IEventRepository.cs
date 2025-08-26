using GuestRegistration.Core.Entities;

namespace GuestRegistration.Core.Interfaces;

public interface IEventRepository
{
    Task<IEnumerable<Event>> GetEventsAsync(bool showFutureEvents);
    Task<Event?> GetByIdAsync(long id);
    Task<Event?> GetByIdWithParticipantsAsync(long id);
    Task<bool> RemoveParticipantAsync(long eventId, long participantId);
    Task DeleteEventAsync(long id);
    Task AddAsync(Event newEvent);
}