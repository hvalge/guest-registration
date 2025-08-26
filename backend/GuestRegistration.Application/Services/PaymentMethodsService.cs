using GuestRegistration.Core.Entities;
using GuestRegistration.Core.Interfaces;

namespace GuestRegistration.Application.Services;

public class PaymentMethodsService
{
    private readonly IPaymentMethodsRepository _paymentMethodsRepository;

    public PaymentMethodsService(IPaymentMethodsRepository paymentMethodsRepository)
    {
        _paymentMethodsRepository = paymentMethodsRepository;
    }
    
    public async Task<List<PaymentMethod>> GetPaymentMethodsAsync()
    {
        return await _paymentMethodsRepository.GetPaymentMethodsAsync();
    }
}