using GuestRegistration.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GuestRegistration.Api.Controllers;

[ApiController]
[Route("api")]
public class PaymentMethodsController : ControllerBase
{
    private readonly PaymentMethodsService _paymentMethodsService;

    public PaymentMethodsController(PaymentMethodsService paymentMethodsService)
    {
        _paymentMethodsService = paymentMethodsService;
    }

    [HttpGet("payment-methods")]
    public async Task<IActionResult> GetPaymentMethods()
    {
        var paymentMethods = await _paymentMethodsService.GetPaymentMethodsAsync();
        return Ok(paymentMethods);
    }
}

