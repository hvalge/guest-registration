using GuestRegistration.Core.Entities;
using GuestRegistration.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GuestRegistration.Infrastructure.Persistence.Repositories;

public class ParticipantRepository : IParticipantRepository
{
    private readonly ApplicationDbContext _context;

    public ParticipantRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Participant participant)
    {
        await _context.Participants.AddAsync(participant);
        await _context.SaveChangesAsync();
    }

    public async Task<EventParticipant?> GetEventParticipantAsync(long eventId, long participantId)
    {
        return await _context.EventParticipants
            .Include(ep => ep.Participant)
            .FirstOrDefaultAsync(ep => ep.EventId == eventId && ep.ParticipantId == participantId);
    }

    public async Task UpdateAsync(EventParticipant eventParticipant)
    {
        _context.EventParticipants.Update(eventParticipant);
        await _context.SaveChangesAsync();
    }
}