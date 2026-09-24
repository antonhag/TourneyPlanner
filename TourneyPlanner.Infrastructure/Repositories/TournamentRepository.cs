using TourneyPlanner.Domain.Entities;
using TourneyPlanner.Application.Interfaces;
using TourneyPlanner.Application.Interfaces.Repositories;

namespace TourneyPlanner.Infrastructure.Repositories;

public class TournamentRepository : ITournamentRepository
{
    
    public async Task AddAsync(Tournament tournament)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Tournament>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task UpdateAsync(Tournament tournament)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(Tournament tournament)
    {
        throw new NotImplementedException();
    }

    public async Task<Tournament?> GetByIdWithParticipantAsync(int id)
    {
        throw new NotImplementedException();
    }
}