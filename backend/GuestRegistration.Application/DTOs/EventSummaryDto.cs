namespace GuestRegistration.Application.DTOs;

public class EventSummaryDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public string Location { get; set; } = string.Empty;
    public int ParticipantCount { get; set; }
}