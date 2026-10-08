using TourneyPlanner.Application.DTOs;

namespace TourneyPlanner.Application.Interfaces.Services;

public interface IMatchService
{
    Task GenerateScheduleAsync(int tournamentId);
    
    Task UpdateScheduleAsync(int tournamentId);

    Task<List<MatchDto>> GetAllScheduleAsync(int tournamentId);
    
    Task RegisterResultAsync(int matchId, UpdateMatchResultDto dto);
    
}