using GuestRegistration.Application.Services;
using GuestRegistration.Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace GuestRegistration.Api.Controllers;

[ApiController]
[Route("api")]
[Produces("application/json")]
public class PaymentMethodsController : ControllerBase
{
    private readonly PaymentMethodsService _paymentMethodsService;

    public PaymentMethodsController(PaymentMethodsService paymentMethodsService)
    {
        _paymentMethodsService = paymentMethodsService;
    }
    
    [HttpGet("payment-methods")]
    [ProducesResponseType(typeof(List<PaymentMethod>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaymentMethods()
    {
        var paymentMethods = await _paymentMethodsService.GetPaymentMethodsAsync();
        return Ok(paymentMethods);
    }
}