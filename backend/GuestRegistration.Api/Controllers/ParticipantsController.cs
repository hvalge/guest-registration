using GuestRegistration.Application.DTOs;
using GuestRegistration.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GuestRegistration.Api.Controllers;

[ApiController]
[Route("api")]
public class ParticipantsController : ControllerBase
{
    private readonly ParticipantService _participantService;
    private readonly ILogger<ParticipantsController> _logger;
    
    public ParticipantsController(ParticipantService participantService, ILogger<ParticipantsController> logger)
    {
        _participantService = participantService;
        _logger = logger;
    }

    [HttpGet("events/{eventId}/participants/{participantId}")]
    public async Task<IActionResult> GetParticipantDetails(long eventId, long participantId)
    {
        var details = await _participantService.GetParticipantDetailsAsync(eventId, participantId);
        if (details == null) return NotFound();
        return Ok(details);
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
            _logger.LogWarning(ex, "Failed to add participant for event {EventId}", eventId);
            return BadRequest(ex.Message);
        }
    }
    
    [HttpPut("events/{eventId}/participants/{participantId}")]
    public async Task<IActionResult> UpdateParticipant(long eventId, long participantId, [FromBody] UpdateParticipantDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
            
        try
        {
            await _participantService.UpdateParticipantDetailsAsync(eventId, participantId, dto);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}