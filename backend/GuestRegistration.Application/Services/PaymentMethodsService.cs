using GuestRegistration.Core.Entities;
using GuestRegistration.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace GuestRegistration.Application.Services;

public class PaymentMethodsService
{
    private readonly IPaymentMethodsRepository _paymentMethodsRepository;
    private readonly ILogger<PaymentMethodsService> _logger;

    public PaymentMethodsService(IPaymentMethodsRepository paymentMethodsRepository, ILogger<PaymentMethodsService> logger)
    {
        _paymentMethodsRepository = paymentMethodsRepository;
        _logger = logger;
    }
    
    public async Task<List<PaymentMethod>> GetPaymentMethodsAsync()
    {
        _logger.LogInformation("Fetching payment methods");
        return await _paymentMethodsRepository.GetPaymentMethodsAsync();
    }
}