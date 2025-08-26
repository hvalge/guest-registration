using GuestRegistration.Application.DTOs;
using GuestRegistration.Application.Exceptions;
using GuestRegistration.Application.Services;
using GuestRegistration.Core.Entities;
using GuestRegistration.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GuestRegistration.Tests.UnitTests;

public class EventServiceTests
{
    private readonly Mock<IEventRepository> _eventRepositoryMock;
    private readonly EventService _eventService;

    public EventServiceTests()
    {
        _eventRepositoryMock = new Mock<IEventRepository>();
        var loggerMock = new Mock<ILogger<EventService>>();
        _eventService = new EventService(_eventRepositoryMock.Object, loggerMock.Object);
    }

    [Fact]
    public async Task GetEventsAsync_ReturnsCorrectEvents()
    {
        var futureEvents = new List<Event> { new() { Name = "Future Event" } };
        var pastEvents = new List<Event> { new() { Name = "Past Event" } };
        _eventRepositoryMock.Setup(r => r.GetEventsAsync(true)).ReturnsAsync(futureEvents);
        _eventRepositoryMock.Setup(r => r.GetEventsAsync(false)).ReturnsAsync(pastEvents);
        
        var futureResult = await _eventService.GetEventsAsync(true);
        var pastResult = await _eventService.GetEventsAsync(false);
        
        var eventSummaryDtos = futureResult as EventSummaryDto[] ?? futureResult.ToArray();
        Assert.Single(eventSummaryDtos);
        Assert.Equal("Future Event", eventSummaryDtos.First().Name);
        var summaryDtos = pastResult as EventSummaryDto[] ?? pastResult.ToArray();
        Assert.Single(summaryDtos);
        Assert.Equal("Past Event", summaryDtos.First().Name);
    }

    [Fact]
    public async Task GetEventDetailsAsync_WhenEventNotFound_ReturnsNull()
    {
        _eventRepositoryMock.Setup(r => r.GetByIdWithParticipantsAsync(It.IsAny<long>())).ReturnsAsync((Event)null!);
        
        var result = await _eventService.GetEventDetailsAsync(1);
        
        Assert.Null(result);
    }

    [Fact]
    public async Task GetEventDetailsAsync_WhenEventFound_ReturnsDetails()
    {
        var eventEntity = new Event { Id = 1, Name = "Test Event" };
        eventEntity.EventParticipants.Add(new EventParticipant { Participant = new NaturalPerson { FirstName = "John", LastName = "Doe", IdCode = "123" } });
        _eventRepositoryMock.Setup(r => r.GetByIdWithParticipantsAsync(1)).ReturnsAsync(eventEntity);
        
        var result = await _eventService.GetEventDetailsAsync(1);
        
        Assert.NotNull(result);
        Assert.Equal("Test Event", result.Name);
        Assert.Single(result.Participants);
        Assert.Equal("John Doe", result.Participants.First().Name);
    }


    [Fact]
    public async Task CreateEventAsync_WithPastStartTime_ThrowsValidationException()
    {
        var createEventDto = new CreateEventDto { StartTime = DateTime.UtcNow.AddDays(-1) };
        
        await Assert.ThrowsAsync<ValidationException>(() => _eventService.CreateEventAsync(createEventDto));
    }

    [Fact]
    public async Task CreateEventAsync_WithValidData_CreatesEvent()
    {
        var createEventDto = new CreateEventDto
        {
            Name = "New Event",
            StartTime = DateTime.UtcNow.AddDays(1),
            Location = "Here",
            AdditionalInformation = "Info"
        };
        
        _eventRepositoryMock.Setup(r => r.GetByIdWithParticipantsAsync(It.IsAny<long>())).ReturnsAsync(new Event());

        
        await _eventService.CreateEventAsync(createEventDto);
        
        _eventRepositoryMock.Verify(r => r.AddAsync(It.Is<Event>(e => e.Name == "New Event")), Times.Once);
    }

    [Fact]
    public async Task DeleteEventAsync_WhenEventNotFound_ThrowsNotFoundException()
    {
        _eventRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<long>())).ReturnsAsync((Event)null!);
        
        await Assert.ThrowsAsync<NotFoundException>(() => _eventService.DeleteEventAsync(1));
    }

    [Fact]
    public async Task DeleteEventAsync_WhenEventHasStarted_ThrowsBusinessRuleValidationException()
    {
        var pastEvent = new Event { StartTime = DateTime.UtcNow.AddDays(-1) };
        _eventRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(pastEvent);
        
        await Assert.ThrowsAsync<BusinessRuleValidationException>(() => _eventService.DeleteEventAsync(1));
    }

    [Fact]
    public async Task DeleteEventAsync_WithFutureEvent_DeletesSuccessfully()
    {
        var futureEvent = new Event { Id = 1, StartTime = DateTime.UtcNow.AddDays(1) };
        _eventRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(futureEvent);
        
        await _eventService.DeleteEventAsync(1);
        
        _eventRepositoryMock.Verify(r => r.DeleteEventAsync(1), Times.Once);
    }
}