using GuestRegistration.Application.DTOs;
using GuestRegistration.Application.Exceptions;
using GuestRegistration.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GuestRegistration.Api.Controllers;

[ApiController]
[Route("api/events/{eventId}/participants")]
public class ParticipantsController : ControllerBase
{
    private readonly ParticipantService _participantService;

    public ParticipantsController(ParticipantService participantService)
    {
        _participantService = participantService;
    }

    [HttpGet("{participantId}")]
    [ProducesResponseType(typeof(ParticipantDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetParticipantDetails(long eventId, long participantId)
    {
        var details = await _participantService.GetParticipantDetailsAsync(eventId, participantId);
        if (details == null)
        {
            return NotFound($"Participant {participantId} not found in event {eventId}.");
        }
        return Ok(details);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ParticipantDetailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddParticipant(long eventId, [FromBody] CreateParticipantDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var newParticipant = await _participantService.AddParticipantToEventAsync(eventId, dto);
        
        return CreatedAtAction(
            nameof(GetParticipantDetails), 
            new { eventId = eventId, participantId = newParticipant.Id }, 
            newParticipant);
    }

    [HttpPut("{participantId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateParticipant(long eventId, long participantId, [FromBody] UpdateParticipantDto dto)
    {
        try
        {
            await _participantService.UpdateParticipantDetailsAsync(eventId, participantId, dto);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{participantId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteParticipant(long eventId, long participantId)
    {
        await _participantService.RemoveParticipantFromEventAsync(eventId, participantId);
        return NoContent();
    }
}
