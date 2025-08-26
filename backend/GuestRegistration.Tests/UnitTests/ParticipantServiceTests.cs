using GuestRegistration.Application.DTOs;
using GuestRegistration.Application.Services;
using GuestRegistration.Core.Entities;
using GuestRegistration.Core.Enums;
using GuestRegistration.Core.Interfaces;
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
        _participantService = new ParticipantService(_participantRepositoryMock.Object, _eventRepositoryMock.Object);
    }

    [Fact]
    public async Task AddParticipantToEventAsync_WithInvalidIdCode_ThrowsArgumentException()
    {
        var eventId = 1L;
        var dto = new CreateParticipantDto
        {
            Type = ParticipantType.NaturalPerson,
            FirstName = "Test",
            LastName = "User",
            IdCode = "12345678901",
            PaymentMethodId = 1
        };

        _eventRepositoryMock.Setup(r => r.GetByIdAsync(eventId)).ReturnsAsync(new Event());
        
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
            _participantService.AddParticipantToEventAsync(eventId, dto));
        
        Assert.Equal("Invalid Estonian ID code.", exception.Message);
    }

    [Fact]
    public async Task AddParticipantToEventAsync_WithValidIdCode_AddsParticipant()
    {
        var eventId = 1L;
        var dto = new CreateParticipantDto
        {
            Type = ParticipantType.NaturalPerson,
            FirstName = "Mari",
            LastName = "Maasikas",
            IdCode = "49001010230",
            PaymentMethodId = 1
        };

        var eventEntity = new Event { Id = eventId };
        _eventRepositoryMock.Setup(r => r.GetByIdAsync(eventId)).ReturnsAsync(eventEntity);
        _participantRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Participant>())).Returns(Task.CompletedTask);
        
        await _participantService.AddParticipantToEventAsync(eventId, dto);
        
        _participantRepositoryMock.Verify(r => r.AddAsync(It.Is<Participant>(p => 
            p is NaturalPerson && ((NaturalPerson)p).IdCode == dto.IdCode)), Times.Once);
        
        Assert.Single(eventEntity.EventParticipants);
    }
}
