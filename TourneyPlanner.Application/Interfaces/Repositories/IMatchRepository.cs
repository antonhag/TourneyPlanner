using TourneyPlanner.Domain.Entities;

namespace TourneyPlanner.Application.Interfaces.Repositories;

public interface IMatchRepository
{
    Task<bool> ExistsForTournamentAsync(int tournamentId);
    Task AddRangeAsync(List<Match> matches);
}