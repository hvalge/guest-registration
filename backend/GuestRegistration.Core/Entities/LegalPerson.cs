namespace GuestRegistration.Core.Entities;

public class LegalPerson : Participant
{
    public string CompanyName { get; set; } = string.Empty;
    public string RegisterCode { get; set; } = string.Empty;
}