using TourneyPlanner.Application.Interfaces.Repositories;
using TourneyPlanner.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using TourneyPlanner.Domain.Entities;

namespace TourneyPlanner.Infrastructure.Repositories;

public class MatchRepository : IMatchRepository
{
    private readonly TourneyPlannerDbContext _context;
    
    public MatchRepository(TourneyPlannerDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsForTournamentAsync(int tournamentId)
    {
        return await _context.Matches.AnyAsync(m => m.TournamentId == tournamentId );
    }

    public async Task AddRangeAsync(List<Match> matches)
    {
        _context.Matches.AddRange(matches);
        await _context.SaveChangesAsync();
    }
}