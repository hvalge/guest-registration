namespace GuestRegistration.Core.Entities;

public class NaturalPerson : Participant
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string IdCode { get; set; } = string.Empty;
}
