using Microsoft.EntityFrameworkCore;
using TourneyPlanner.Domain.Entities;

namespace TourneyPlanner.Infrastructure.Data;

public class TourneyPlannerDbContext : DbContext
{
    public TourneyPlannerDbContext(DbContextOptions<TourneyPlannerDbContext> options) : base(options)
    {
    }
    
    public DbSet<Tournament> Tournaments { get; set; }
    public DbSet<Participant> Participants { get; set; }
}