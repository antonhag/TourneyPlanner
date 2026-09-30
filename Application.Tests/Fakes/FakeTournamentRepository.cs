using TourneyPlanner.Application.Interfaces.Repositories;
using TourneyPlanner.Domain.Entities;

namespace Application.Tests.Fakes;

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
        return Task.FromResult(Tournaments.ToList());
    }

    public Task UpdateAsync(Tournament tournament)
    {
        var index = Tournaments.FindIndex(t => t.Id == tournament.Id);

        if (index >= 0)
        {
            Tournaments[index] = tournament;
        }
        
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Tournament tournament)
    {
        Tournaments.Remove(tournament);
        return Task.CompletedTask;
    }

    public Task<Tournament?> GetByIdAsync(int id)
    {
        return Task.FromResult(Tournaments.FirstOrDefault(t => t.Id == id));
    }
}