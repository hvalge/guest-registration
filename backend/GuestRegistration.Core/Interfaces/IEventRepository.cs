using GuestRegistration.Core.Entities;

namespace GuestRegistration.Core.Interfaces;

public interface IEventRepository
{
    Task<IEnumerable<Event>> GetEventsAsync(bool showFutureEvents);
}