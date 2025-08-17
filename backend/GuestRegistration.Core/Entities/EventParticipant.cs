namespace GuestRegistration.Core.Entities;

public class EventParticipant
{
    public long EventId { get; set; }
    public Event Event { get; set; } = null!;

    public long ParticipantId { get; set; }
    public Participant Participant { get; set; } = null!;
    
    public long PaymentMethodId { get; set; }
    public PaymentMethod PaymentMethod { get; set; } = null!;
    
    // Only for LegalPerson
    public int? NumberOfAttendees { get; set; } 
    public string? AdditionalInformation { get; set; }
}