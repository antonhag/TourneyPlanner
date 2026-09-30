namespace TourneyPlanner.Application.Interfaces.Services;

public interface IMatchService
{
    Task GenerateScheduleAsync(int tournamentId);
}