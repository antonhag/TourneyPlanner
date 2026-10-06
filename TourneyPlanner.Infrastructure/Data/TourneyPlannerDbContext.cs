using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TourneyPlanner.Domain.Entities;

namespace TourneyPlanner.Infrastructure.Data;

public class TourneyPlannerDbContext : IdentityDbContext<IdentityUser>
{
    public TourneyPlannerDbContext(DbContextOptions<TourneyPlannerDbContext> options) : base(options)
    {
    }
    
    public DbSet<Tournament> Tournaments { get; set; }
    public DbSet<Participant> Participants { get; set; }
    public DbSet<Match> Matches { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Match>(entity =>
        {
            // En match tillhör en turnering, om turneringen tas bort så tas matcherna också bort. 
            entity.HasOne<Tournament>()
                .WithMany()
                .HasForeignKey(m => m.TournamentId);
            
            // Hindrar att en deltagare som har match från att tas bort
            entity.HasOne<Participant>()
                .WithMany()
                .HasForeignKey(m => m.HomeParticipantId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne<Participant>()
                .WithMany()
                .HasForeignKey(m => m.AwayParticipantId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}