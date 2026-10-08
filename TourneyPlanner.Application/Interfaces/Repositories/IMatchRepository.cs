using TourneyPlanner.Domain.Entities;

namespace TourneyPlanner.Application.Interfaces.Repositories;

public interface IMatchRepository
{
    Task<bool> ExistsForTournamentAsync(int tournamentId);
    Task AddRangeAsync(List<Match> matches);
    Task RemoveRangeAsync(List<Match> matches);
    Task<List<Match>> GetTournamentMatchesAsync(int tournamentId);
    Task<Match?> GetByIdAsync(int id);
    Task UpdateAsync(Match match);
}