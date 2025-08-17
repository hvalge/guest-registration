using GuestRegistration.Core.Entities;
using GuestRegistration.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GuestRegistration.Infrastructure.Persistence.Repositories;

public class EventRepository : IEventRepository
{
    private readonly ApplicationDbContext _context;

    public EventRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Event>> GetEventsAsync(bool showFutureEvents)
    {
        var now = DateTime.UtcNow;

        IQueryable<Event> query = _context.Events
            .AsNoTracking()
            .Include(e => e.EventParticipants);

        if (showFutureEvents)
        {
            query = query.Where(e => e.StartTime >= now).OrderBy(e => e.StartTime);
        }
        else
        {
            query = query.Where(e => e.StartTime < now).OrderByDescending(e => e.StartTime);
        }

        return await query.ToListAsync();
    }
}