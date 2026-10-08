using TourneyPlanner.Application.DTOs;

namespace TourneyPlanner.Application.Interfaces.Services;

public interface IStandingService
{
    Task<List<StandingDto>> GetStandingsAsync(int tournamentId);
}