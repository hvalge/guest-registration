namespace GuestRegistration.Core.Entities;

public class Event
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public string Location { get; set; } = string.Empty;
    public string? AdditionalInformation { get; set; }
    
    public ICollection<EventParticipant> EventParticipants { get; set; } = new List<EventParticipant>();
}