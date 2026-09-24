using TourneyPlanner.Application.DTOs;
using TourneyPlanner.Domain.Entities;

namespace TourneyPlanner.Application.Interfaces.Services;

public interface ITournamentService
{
    Task CreateTournamentAsync(CreateTournamentDto dto);
}