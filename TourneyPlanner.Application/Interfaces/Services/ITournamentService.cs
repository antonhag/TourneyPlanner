using TourneyPlanner.Application.DTOs;
using TourneyPlanner.Domain.Entities;

namespace TourneyPlanner.Application.Interfaces.Services;

public interface ITournamentService
{
    Task CreateTournamentAsync(CreateTournamentDto dto);
    Task<List<TournamentDto>> GetAllTournamentsAsync();
    Task<TournamentDto> GetTournamentByIdAsync(int id);
    Task UpdateTournamentAsync(int id, UpdateTournamentDto dto);
    Task DeleteTournamentAsync(int id);
}