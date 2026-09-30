using TourneyPlanner.Application.DTOs;

namespace TourneyPlanner.Application.Interfaces.Services;

public interface IParticipantService 
{
    Task AddParticipantAsync(int tournamentId, CreateParticipantDto dto);
    Task AddParticipantsAsync(int tournamentId, List<CreateParticipantDto> dtos);
    Task <List<ParticipantDto>> GetParticipantsByTournamentIdAsync(int tournamentId);
    Task<ParticipantDto> GetParticipantByIdAsync(int id);
    Task UpdateParticipantAsync(int id, UpdateParticipantDto dto);
    Task DeleteParticipantAsync(int id);
}