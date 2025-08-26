namespace GuestRegistration.Application.Exceptions;

public class BusinessRuleValidationException : ApplicationException
{
    public BusinessRuleValidationException(string message) : base(message)
    {
    }

    public BusinessRuleValidationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}