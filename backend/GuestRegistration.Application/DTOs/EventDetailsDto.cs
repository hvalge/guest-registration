namespace GuestRegistration.Application.DTOs
{
    public class EventDetailsDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<ParticipantDto> Participants { get; set; } = new();
    }
}