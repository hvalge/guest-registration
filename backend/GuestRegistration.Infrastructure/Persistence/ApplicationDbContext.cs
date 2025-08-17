using GuestRegistration.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace GuestRegistration.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    public DbSet<Event> Events { get; set; }
    public DbSet<Participant> Participants { get; set; }
    public DbSet<NaturalPerson> NaturalPersons { get; set; }
    public DbSet<LegalPerson> LegalPersons { get; set; }
    public DbSet<EventParticipant> EventParticipants { get; set; }
    public DbSet<PaymentMethod> PaymentMethods { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<EventParticipant>()
            .HasKey(ep => new { ep.EventId, ep.ParticipantId });

        modelBuilder.Entity<EventParticipant>()
            .HasOne(ep => ep.Event)
            .WithMany(e => e.EventParticipants)
            .HasForeignKey(ep => ep.EventId);

        modelBuilder.Entity<EventParticipant>()
            .HasOne(ep => ep.Participant)
            .WithMany(p => p.EventParticipants)
            .HasForeignKey(ep => ep.ParticipantId);
        
        SeedData(modelBuilder);
    }
    
    private void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Event>().HasData(
            new Event
            {
                Id = 1L,
                Name = "Summer Tech Conference",
                StartTime = DateTime.UtcNow.AddDays(30),
                Location = "Tech Park Tallinn"
            },
            new Event
            {
                Id = 2L,
                Name = "Agile Development Workshop",
                StartTime = DateTime.UtcNow.AddDays(90),
                Location = "Virtual Event"
            },
            new Event
            {
                Id = 3L,
                Name = "Winter Code Retreat",
                StartTime = DateTime.UtcNow.AddDays(-60),
                Location = "Pärnu Hotel"
            },
            new Event
            {
                Id = 4L,
                Name = "Project Management Meetup",
                StartTime = DateTime.UtcNow.AddDays(-120),
                Location = "Tartu University"
            }
        );
    }
}