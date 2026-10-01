using TourneyPlanner.Application.Interfaces.Repositories;
using TourneyPlanner.Domain.Entities;

namespace Application.Tests.Fakes;

public class FakeMatchRepository : IMatchRepository
{
    public List<Match> Matches { get; } = new();

    public Task<Match?> GetByIdAsync(int id)
    {
        return Task.FromResult(Matches.FirstOrDefault(m => m.Id == id));
    }

    public Task<List<Match>> GetByTournamentIdAsync(int tournamentId)
    {
        var result = Matches.Where(m => m.TournamentId == tournamentId).ToList();
        return Task.FromResult(result);
    }

    public Task<bool> ExistsForTournamentAsync(int tournamentId)
    {
        return Task.FromResult(Matches.Any(m => m.TournamentId == tournamentId));
    }

    public Task AddRangeAsync(List<Match> matches)
    {
        foreach (var match in matches)
        {
            match.Id = Matches.Count + 1;
            Matches.Add(match);
        }
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Match match)
    {
        // behövs egentligen inte (interface kräver den dock), objektet i listan är samma som det som skickas in.
        return Task.CompletedTask;
    }
}