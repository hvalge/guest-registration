using GuestRegistration.Core.Enums;

namespace GuestRegistration.Application.DTOs;

public class ParticipantDetailDto
{
    public long Id { get; set; }
    public ParticipantType Type { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? IdCode { get; set; }
    public string? CompanyName { get; set; }
    public string? RegisterCode { get; set; }
    public int? NumberOfAttendees { get; set; }
    public long PaymentMethodId { get; set; }
    public string? AdditionalInformation { get; set; }
}