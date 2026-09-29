using TourneyPlanner.Application.Interfaces.Repositories;
using TourneyPlanner.Infrastructure.Data;

namespace TourneyPlanner.Infrastructure.Repositories;

public class MatchRepository : IMatchRepository
{
    private readonly TourneyPlannerDbContext _context;
    
    public MatchRepository(TourneyPlannerDbContext context)
    {
        _context = context;
    }
}