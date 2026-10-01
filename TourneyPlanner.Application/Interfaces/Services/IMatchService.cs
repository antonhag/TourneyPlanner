using TourneyPlanner.Application.DTOs;

namespace TourneyPlanner.Application.Interfaces.Services;

public interface IMatchService
{
    Task GenerateScheduleAsync(int tournamentId);
    Task<List<MatchDto>> GetMatchesByTournamentIdAsync(int tournamentId);
    Task<MatchDto> GetMatchByIdAsync(int id);
    Task UpdateMatchResultAsync(int id, UpdateMatchResultDto dto);
}