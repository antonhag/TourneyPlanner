using TourneyPlanner.Domain.Entities;

namespace TourneyPlanner.Application.Interfaces.Repositories;

public interface IMatchRepository
{
    Task<Match?> GetByIdAsync(int id);
    Task<List<Match>> GetByTournamentIdAsync(int tournamentId);
    Task<bool> ExistsForTournamentAsync(int tournamentId);

    Task AddRangeAsync(List<Match> matches);
    Task UpdateAsync(Match match);
}