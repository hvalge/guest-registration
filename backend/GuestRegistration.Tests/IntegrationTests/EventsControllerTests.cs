using System.Net;
using System.Text;
using System.Text.Json;
using GuestRegistration.Application.DTOs;
using GuestRegistration.Core.Entities;
using GuestRegistration.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace GuestRegistration.Tests.IntegrationTests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        
        builder.ConfigureServices(services =>
        {
            services.Remove(
                services.SingleOrDefault(d => d.ServiceType == typeof(IDbContextOptionsConfiguration<ApplicationDbContext>))!
            );
            
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            });
        });
    }
}


public class EventsControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public EventsControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }
    
    private async Task SeedData()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        if (!await context.Events.AnyAsync())
        {
            context.Events.AddRange(
                new Event { Name = "Future Event", StartTime = DateTime.UtcNow.AddDays(1), Location = "Test Location" },
                new Event { Name = "Past Event", StartTime = DateTime.UtcNow.AddDays(-1), Location = "Test Location" }
            );
            await context.SaveChangesAsync();
        }
    }

    [Fact]
    public async Task GetEvents_FutureView_ReturnsOnlyFutureEvents()
    {
        await SeedData();
        
        var response = await _client.GetAsync("/api/events?view=future");
        
        response.EnsureSuccessStatusCode();
        var stringResponse = await response.Content.ReadAsStringAsync();
        var events = JsonSerializer.Deserialize<List<EventSummaryDto>>(stringResponse, 
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(events);
        Assert.Single(events);
        Assert.Equal("Future Event", events.First().Name);
    }
    
    [Fact]
    public async Task CreateEvent_WithValidData_ReturnsOk()
    {
        await SeedData();
        var createEventDto = new CreateEventDto
        {
            Name = "New Test Event",
            StartTime = DateTime.UtcNow.AddDays(10),
            Location = "Virtual",
            AdditionalInformation = "Some info"
        };
        var content = new StringContent(JsonSerializer.Serialize(createEventDto), Encoding.UTF8, "application/json");
        
        var response = await _client.PostAsync("/api/events", content);
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
