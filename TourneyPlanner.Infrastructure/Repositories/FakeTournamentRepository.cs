using TourneyPlanner.Application.Interfaces.Repositories;
using TourneyPlanner.Domain.Entities;

namespace TourneyPlanner.Infrastructure.Repositories;

public class FakeTournamentRepository : ITournamentRepository
{
    public List<Tournament> Tournaments { get; } = new();
    
    public Task AddAsync(Tournament tournament)
    {
       tournament.Id = Tournaments.Count + 1;
       Tournaments.Add(tournament);
       return Task.CompletedTask;
    }

    public Task<List<Tournament>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Tournament tournament)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Tournament tournament)
    {
        throw new NotImplementedException();
    }
}