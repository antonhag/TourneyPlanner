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

    public async Task<Match?> GetByIdAsync(int id)
    {
        return await _context.Matches.FindAsync(id);
    }

    public async Task<List<Match>> GetByTournamentIdAsync(int tournamentId)
    {
        // sorteras per runda så att schemat kommer i spelordning
        return await _context.Matches
            .Where(m => m.TournamentId == tournamentId)
            .OrderBy(m => m.Round)
            .ThenBy(m => m.Id)
            .ToListAsync();
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

    public async Task UpdateAsync(Match match)
    {
        _context.Matches.Update(match);
        await _context.SaveChangesAsync();
    }
}