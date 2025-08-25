using System.ComponentModel.DataAnnotations;
using GuestRegistration.Core.Enums;

namespace GuestRegistration.Application.DTOs;

public class CreateParticipantDto
{
    [Required]
    public ParticipantType Type { get; set; }
        
    [RequiredIf(nameof(Type), ParticipantType.NaturalPerson)]
    [StringLength(50)]
    public string? FirstName { get; set; }

    [RequiredIf(nameof(Type), ParticipantType.NaturalPerson)]
    [StringLength(50)]
    public string? LastName { get; set; }

    [RequiredIf(nameof(Type), ParticipantType.NaturalPerson)]
    [RegularExpression(@"^[1-6]\d{2}(0[1-9]|1[0-2])(0[1-9]|[12]\d|3[01])\d{4}$")]
    public string? IdCode { get; set; }
        
    [RequiredIf(nameof(Type), ParticipantType.LegalPerson)]
    [StringLength(100)]
    public string? CompanyName { get; set; }

    [RequiredIf(nameof(Type), ParticipantType.LegalPerson)]
    [RegularExpression(@"^\d{8}$")]
    public string? RegisterCode { get; set; }
        
    [Range(1, int.MaxValue)]
    public int? NumberOfAttendees { get; set; }
        
    [Required]
    public long PaymentMethodId { get; set; }

    [StringLength(1500, ErrorMessage = "Additional information cannot be longer than 1500 characters for a natural person.")]
    public string? AdditionalInformationNatural { get; set; }
        
    [StringLength(5000, ErrorMessage = "Additional information cannot be longer than 5000 characters for a legal person.")]
    public string? AdditionalInformationLegal { get; set; }
}
    
public class RequiredIfAttribute : ValidationAttribute
{
    private readonly string _propertyName;
    private readonly object _expectedValue;

    public RequiredIfAttribute(string propertyName, object expectedValue)
    {
        _propertyName = propertyName;
        _expectedValue = expectedValue;
    }

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        var instance = validationContext.ObjectInstance;
        var property = instance.GetType().GetProperty(_propertyName);
        if (property != null)
        {
            var propertyValue = property.GetValue(instance);
            if (propertyValue?.ToString() == _expectedValue.ToString())
            {
                if (string.IsNullOrWhiteSpace(value?.ToString()))
                {
                    return new ValidationResult(ErrorMessage);
                }
            }
        }
        return ValidationResult.Success!;
    }
}