using System.ComponentModel.DataAnnotations;
using GuestRegistration.Core.Enums;

namespace GuestRegistration.Application.DTOs;

public class UpdateParticipantDto
{
    [Required]
    public ParticipantType Type { get; set; }
        
    [RequiredIf(nameof(Type), ParticipantType.NaturalPerson)]
    public string? FirstName { get; set; }
    [RequiredIf(nameof(Type), ParticipantType.NaturalPerson)]
    public string? LastName { get; set; }
    [RequiredIf(nameof(Type), ParticipantType.NaturalPerson)]
    public string? IdCode { get; set; }
        
    [RequiredIf(nameof(Type), ParticipantType.LegalPerson)]
    public string? CompanyName { get; set; }
    [RequiredIf(nameof(Type), ParticipantType.LegalPerson)]
    public string? RegisterCode { get; set; }
    public int? NumberOfAttendees { get; set; }
        
    [Required]
    public long PaymentMethodId { get; set; }
    public string? AdditionalInformation { get; set; }
}