using GuestRegistration.Core.Entities;

namespace GuestRegistration.Core.Interfaces;

public interface IPaymentMethodsRepository
{
    Task<List<PaymentMethod>> GetPaymentMethodsAsync();
}