using GuestRegistration.Core.Entities;
using GuestRegistration.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GuestRegistration.Infrastructure.Seed;

public static class DataSeeder
{
    public static async Task SeedAsync(IHost app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        await context.Database.MigrateAsync();
        
        if (await context.Events.AnyAsync())
        {
            return; 
        }

        var events = new List<Event>
        {
            new Event
            {
                Name = "Summer Tech Conference",
                StartTime = DateTime.UtcNow.AddDays(30),
                Location = "Tech Park Tallinn"
            },
            new Event
            {
                Name = "Agile Development Workshop",
                StartTime = DateTime.UtcNow.AddDays(90),
                Location = "Virtual Event"
            },
            new Event
            {
                Name = "Winter Code Retreat",
                StartTime = DateTime.UtcNow.AddDays(-60),
                Location = "Pärnu Hotel"
            },
            new Event
            {
                Name = "Project Management Meetup",
                StartTime = DateTime.UtcNow.AddDays(-120),
                Location = "Tartu University"
            }
        };

        await context.Events.AddRangeAsync(events);
        await context.SaveChangesAsync();
    }
}