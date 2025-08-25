using GuestRegistration.Application.DTOs;
using GuestRegistration.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GuestRegistration.Api.Controllers;

[ApiController]
[Route("api")]
public class ParticipantsController : ControllerBase
{
    private readonly ParticipantService _participantService;
    
    public ParticipantsController(ParticipantService participantService)
    {
        _participantService = participantService;
    }

    [HttpPost("events/{eventId}/participants")]
    public async Task<IActionResult> AddParticipant(long eventId, [FromBody] CreateParticipantDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _participantService.AddParticipantToEventAsync(eventId, dto);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("payment-methods")]
    public async Task<IActionResult> GetPaymentMethods()
    {
        var paymentMethods = await _participantService.GetPaymentMethodsAsync();
        return Ok(paymentMethods);
    }
}