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
        
    }
}