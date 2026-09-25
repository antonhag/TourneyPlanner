using Microsoft.EntityFrameworkCore;
using TourneyPlanner.Domain.Entities;
using TourneyPlanner.Application.Interfaces;
using TourneyPlanner.Application.Interfaces.Repositories;
using TourneyPlanner.Infrastructure.Data;

namespace TourneyPlanner.Infrastructure.Repositories;

public class TournamentRepository : ITournamentRepository
{
    private readonly TourneyPlannerDbContext _context;
    
    public TournamentRepository(TourneyPlannerDbContext context)
    {
        _context = context;
    }
    
    public async Task AddAsync(Tournament tournament)
    {
        _context.Tournaments.Add(tournament);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Tournament>> GetAllAsync()
    {
        return await _context.Tournaments.ToListAsync();
    }

    public async Task UpdateAsync(Tournament tournament)
    {
        _context.Tournaments.Update(tournament);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Tournament tournament)
    {
        _context.Tournaments.Remove(tournament);
        await _context.SaveChangesAsync();
    }
}