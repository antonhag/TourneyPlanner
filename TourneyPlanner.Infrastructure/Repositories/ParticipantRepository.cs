using TourneyPlanner.Domain.Entities;
using TourneyPlanner.Application.Interfaces;
using TourneyPlanner.Application.Interfaces.Repositories;
using TourneyPlanner.Infrastructure.Data;

namespace TourneyPlanner.Infrastructure.Repositories;

public class ParticipantRepository : IParticipantRepository
{
    private readonly TourneyPlannerDbContext _context;

    public ParticipantRepository(TourneyPlannerDbContext context)
    {
        _context = context;
    }
    
    public async Task<Participant?> GetParticipantById(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Participant>> GetByTournamentIdAsync(int tournamentId)
    {
        throw new NotImplementedException();
    }

    public async Task AddAsync(Participant participant)
    {
        throw new NotImplementedException();
    }

    public async Task AddRangeAsync(List<Participant> participants)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateAsync(Participant participant)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}