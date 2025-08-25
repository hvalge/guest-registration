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
        
        var paymentMethod1 = new PaymentMethod { Name = "Pangaülekanne" };
        var paymentMethod2 = new PaymentMethod { Name = "Sularaha" };
        
        var naturalPerson1 = new NaturalPerson { FirstName = "Mari", LastName = "Maasikas", IdCode = "49001011234" };
        var naturalPerson2 = new NaturalPerson { FirstName = "Kalle", LastName = "Kask", IdCode = "38502022345" };
        var legalPerson1 = new LegalPerson { CompanyName = "Innovatsioon OÜ", RegisterCode = "12345678" };
        var legalPerson2 = new LegalPerson { CompanyName = "Arendus AS", RegisterCode = "87654321" };
        
        var events = new List<Event>
        {
            new()
            {
                Name = "Suvine Tehnoloogiakonverents",
                StartTime = DateTime.UtcNow.AddDays(30),
                Location = "Tehnopol, Tallinn",
                EventParticipants = new List<EventParticipant>
                {
                    new() { Participant = naturalPerson1, PaymentMethod = paymentMethod1 },
                    new() { Participant = legalPerson1, PaymentMethod = paymentMethod1, NumberOfAttendees = 5 }
                }
            },
            new()
            {
                Name = "Agiilse Arenduse Töötuba",
                StartTime = DateTime.UtcNow.AddDays(90),
                Location = "Virtuaalne üritus",
                 EventParticipants = new List<EventParticipant>
                {
                    new() { Participant = naturalPerson2, PaymentMethod = paymentMethod2 },
                    new() { Participant = legalPerson2, PaymentMethod = paymentMethod1, NumberOfAttendees = 2 }
                }
            },
            new()
            {
                Name = "Talvine Häkaton",
                StartTime = DateTime.UtcNow.AddDays(-60),
                Location = "Pärnu hotell",
                EventParticipants = new List<EventParticipant>
                {
                    new() { Participant = naturalPerson2, PaymentMethod = paymentMethod2 }
                }
            },
            new()
            {
                Name = "Projektijuhtimise Meetup",
                StartTime = DateTime.UtcNow.AddDays(-120),
                Location = "Tartu Ülikool",
                EventParticipants = new List<EventParticipant>
                {
                    new() { Participant = naturalPerson1, PaymentMethod = paymentMethod1 },
                    new() { Participant = legalPerson2, PaymentMethod = paymentMethod1, NumberOfAttendees = 10 }
                }
            }
        };

        await context.Events.AddRangeAsync(events);
        await context.SaveChangesAsync();
    }
}
