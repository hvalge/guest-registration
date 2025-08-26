using GuestRegistration.Application.DTOs;
using GuestRegistration.Application.Exceptions;
using GuestRegistration.Application.Services;
using GuestRegistration.Core.Entities;
using GuestRegistration.Core.Enums;
using GuestRegistration.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GuestRegistration.Tests.UnitTests;

public class ParticipantServiceTests
{
    private readonly Mock<IParticipantRepository> _participantRepositoryMock;
    private readonly Mock<IEventRepository> _eventRepositoryMock;
    private readonly ParticipantService _participantService;

    public ParticipantServiceTests()
    {
        _participantRepositoryMock = new Mock<IParticipantRepository>();
        _eventRepositoryMock = new Mock<IEventRepository>();
        var loggerMock = new Mock<ILogger<ParticipantService>>();
        
        _participantService = new ParticipantService(
            _participantRepositoryMock.Object, 
            _eventRepositoryMock.Object, 
            loggerMock.Object);
    }

    [Fact]
    public async Task AddParticipantToEventAsync_WhenEventNotFound_ThrowsNotFoundException()
    {
        var eventId = 1L;
        var dto = new CreateParticipantDto { Type = ParticipantType.NaturalPerson, IdCode = "49001010230" };
        _eventRepositoryMock.Setup(r => r.GetByIdAsync(eventId)).ReturnsAsync((Event)null!);

        await Assert.ThrowsAsync<NotFoundException>(() => 
            _participantService.AddParticipantToEventAsync(eventId, dto));
    }

    [Fact]
    public async Task AddParticipantToEventAsync_WithInvalidIdCode_ThrowsValidationException()
    {
        var eventId = 1L;
        var dto = new CreateParticipantDto
        {
            Type = ParticipantType.NaturalPerson,
            FirstName = "Test",
            LastName = "User",
            IdCode = "12345678901", // Invalid checksum
            PaymentMethodId = 1
        };

        _eventRepositoryMock.Setup(r => r.GetByIdAsync(eventId)).ReturnsAsync(new Event());
        
        var exception = await Assert.ThrowsAsync<ValidationException>(() => 
            _participantService.AddParticipantToEventAsync(eventId, dto));
        
        Assert.Equal("Invalid Estonian ID code.", exception.Message);
    }

    [Fact]
    public async Task AddParticipantToEventAsync_WithValidNaturalPerson_AddsParticipant()
    {
        var eventId = 1L;
        var dto = new CreateParticipantDto
        {
            Type = ParticipantType.NaturalPerson,
            FirstName = "Mari",
            LastName = "Maasikas",
            IdCode = "49001010230", // Valid checksum
            PaymentMethodId = 1
        };

        var eventEntity = new Event { Id = eventId };
        _eventRepositoryMock.Setup(r => r.GetByIdAsync(eventId)).ReturnsAsync(eventEntity);
         _participantRepositoryMock.Setup(r => r.GetEventParticipantAsync(eventId, It.IsAny<long>())).ReturnsAsync(new EventParticipant { Participant = new NaturalPerson() });


        await _participantService.AddParticipantToEventAsync(eventId, dto);
        
        _participantRepositoryMock.Verify(r => r.AddAsync(It.Is<Participant>(p => 
            ((NaturalPerson)p).IdCode == dto.IdCode)), Times.Once);
        
        Assert.Single(eventEntity.EventParticipants);
    }
    
    [Fact]
    public async Task AddParticipantToEventAsync_WithValidLegalPerson_AddsParticipant()
    {
        var eventId = 1L;
        var dto = new CreateParticipantDto
        {
            Type = ParticipantType.LegalPerson,
            CompanyName = "Test OÜ",
            RegisterCode = "12345678",
            PaymentMethodId = 1,
            NumberOfAttendees = 5
        };

        var eventEntity = new Event { Id = eventId };
        _eventRepositoryMock.Setup(r => r.GetByIdAsync(eventId)).ReturnsAsync(eventEntity);
        _participantRepositoryMock.Setup(r => r.GetEventParticipantAsync(eventId, It.IsAny<long>())).ReturnsAsync(new EventParticipant { Participant = new LegalPerson() });


        await _participantService.AddParticipantToEventAsync(eventId, dto);

        _participantRepositoryMock.Verify(r => r.AddAsync(It.Is<Participant>(p =>
            ((LegalPerson)p).CompanyName == dto.CompanyName)), Times.Once);

        Assert.Single(eventEntity.EventParticipants);
        Assert.Equal(5, eventEntity.EventParticipants.First().NumberOfAttendees);
    }

    [Fact]
    public async Task GetParticipantDetailsAsync_WhenParticipantNotFound_ReturnsNull()
    {
        var eventId = 1L;
        var participantId = 1L;
        _participantRepositoryMock.Setup(r => r.GetEventParticipantAsync(eventId, participantId))
            .ReturnsAsync((EventParticipant)null!);

        var result = await _participantService.GetParticipantDetailsAsync(eventId, participantId);

        Assert.Null(result);
    }
    
    [Fact]
    public async Task GetParticipantDetailsAsync_ForNaturalPerson_ReturnsCorrectDetails()
    {
        var eventId = 1L;
        var participantId = 1L;
        var eventParticipant = new EventParticipant
        {
            EventId = eventId,
            ParticipantId = participantId,
            PaymentMethodId = 1,
            AdditionalInformation = "Test info",
            Participant = new NaturalPerson { Id = participantId, FirstName = "Test", LastName = "User", IdCode = "49001010230" }
        };

        _participantRepositoryMock.Setup(r => r.GetEventParticipantAsync(eventId, participantId)).ReturnsAsync(eventParticipant);

        var result = await _participantService.GetParticipantDetailsAsync(eventId, participantId);

        Assert.NotNull(result);
        Assert.Equal(ParticipantType.NaturalPerson, result.Type);
        Assert.Equal("Test", result.FirstName);
        Assert.Equal("User", result.LastName);
    }

    [Fact]
    public async Task RemoveParticipantFromEventAsync_WhenParticipantNotFound_ThrowsNotFoundException()
    {
        var eventId = 1L;
        var participantId = 1L;
        _eventRepositoryMock.Setup(r => r.RemoveParticipantAsync(eventId, participantId)).ReturnsAsync(false);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _participantService.RemoveParticipantFromEventAsync(eventId, participantId));
    }

    [Fact]
    public async Task RemoveParticipantFromEventAsync_WhenParticipantExists_RemovesSuccessfully()
    {
        var eventId = 1L;
        var participantId = 1L;
        _eventRepositoryMock.Setup(r => r.RemoveParticipantAsync(eventId, participantId)).ReturnsAsync(true);

        await _participantService.RemoveParticipantFromEventAsync(eventId, participantId);

        _eventRepositoryMock.Verify(r => r.RemoveParticipantAsync(eventId, participantId), Times.Once);
    }

    [Fact]
    public async Task UpdateParticipantDetailsAsync_WhenParticipantNotFound_ThrowsException()
    {
        var eventId = 1L;
        var participantId = 1L;
        var dto = new UpdateParticipantDto();
        _participantRepositoryMock.Setup(r => r.GetEventParticipantAsync(eventId, participantId))
            .ReturnsAsync((EventParticipant)null!);

        await Assert.ThrowsAsync<Exception>(() =>
            _participantService.UpdateParticipantDetailsAsync(eventId, participantId, dto));
    }
    
    [Fact]
    public async Task UpdateParticipantDetailsAsync_UpdatesCorrectly()
    {
        var eventId = 1L;
        var participantId = 1L;
        var eventParticipant = new EventParticipant
        {
            Participant = new NaturalPerson { Id = participantId, FirstName = "Old", LastName = "Name", IdCode = "12345" }
        };
        var dto = new UpdateParticipantDto
        {
            Type = ParticipantType.NaturalPerson,
            FirstName = "New",
            LastName = "Name",
            IdCode = "54321",
            PaymentMethodId = 2,
            AdditionalInformation = "Updated info"
        };

        _participantRepositoryMock.Setup(r => r.GetEventParticipantAsync(eventId, participantId)).ReturnsAsync(eventParticipant);

        await _participantService.UpdateParticipantDetailsAsync(eventId, participantId, dto);

        _participantRepositoryMock.Verify(r => r.UpdateAsync(It.Is<EventParticipant>(ep =>
            ((NaturalPerson)ep.Participant).FirstName == "New" &&
            ep.PaymentMethodId == 2 &&
            ep.AdditionalInformation == "Updated info")), Times.Once);
    }
}