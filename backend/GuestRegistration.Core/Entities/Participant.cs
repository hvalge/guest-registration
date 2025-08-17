namespace GuestRegistration.Core.Entities;

public abstract class Participant
{
    public long Id { get; set; }
    
    public ICollection<EventParticipant> EventParticipants { get; set; } = new List<EventParticipant>();
}
