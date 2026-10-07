using TourneyPlanner.Application.Interfaces.Repositories;
using TourneyPlanner.Domain.Entities;

namespace Application.Tests.Fakes;

public class FakeMatchRepository : IMatchRepository
{
    public List<Match> Matches { get; } = new();

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

    public Task RemoveRangeAsync(List<Match> matches)
    {
        foreach (var match in matches)
        {
            Matches.Remove(match);
        }
        return Task.CompletedTask;
    }

    public Task<List<Match>> GetTournamentMatchesAsync(int tournamentId)
    {
        var result = Matches.Where(m => m.TournamentId == tournamentId).ToList();
        return Task.FromResult(result);
    }
}