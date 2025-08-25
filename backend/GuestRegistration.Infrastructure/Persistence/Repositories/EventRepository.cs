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

    public async Task<Event?> GetByIdAsync(long id)
    {
        return await _context.Events.FindAsync(id);
    }

    public async Task<Event?> GetByIdWithParticipantsAsync(long id)
    {
        return await _context.Events
            .Include(e => e.EventParticipants)
            .ThenInclude(ep => ep.Participant)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task RemoveParticipantAsync(long eventId, long participantId)
    {
        var eventParticipant = await _context.EventParticipants
            .FirstOrDefaultAsync(ep => ep.EventId == eventId && ep.ParticipantId == participantId);

        if (eventParticipant != null)
        {
            _context.EventParticipants.Remove(eventParticipant);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteEventAsync(long id)
    {
        var eventToDelete = await _context.Events.FindAsync(id);
        if (eventToDelete != null)
        {
            _context.Events.Remove(eventToDelete);
            await _context.SaveChangesAsync();
        }
    }

    public async Task AddAsync(Event newEvent)
    {
        await _context.Events.AddAsync(newEvent);
        await _context.SaveChangesAsync();
    }
}