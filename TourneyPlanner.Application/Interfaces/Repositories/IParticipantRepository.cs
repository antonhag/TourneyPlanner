using TourneyPlanner.Domain.Entities;

namespace TourneyPlanner.Application.Interfaces.Repositories;

public interface IParticipantRepository
{
    Task<Participant?> GetParticipantById(int id);
    Task<List<Participant>> GetByTournamentIdAsync(int tournamentId);
    
    Task AddAsync(Participant participant);
    Task AddRangeAsync(List<Participant> participants);
    Task UpdateAsync(Participant participant);
    Task DeleteAsync(int id);
}