using TourneyPlanner.Domain.Entities;
using TourneyPlanner.Application.Interfaces;
using TourneyPlanner.Application.Interfaces.Repositories;

namespace TourneyPlanner.Infrastructure.Repositories;

public class ParticipantRepository : IParticipantRepository
{
    public async Task<Participant?> GetParticipantById(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Participant>> GetByTournamentIdAsync(int tournamentId)
    {
        throw new NotImplementedException();
    }

    public async Task AddAsync(Participant participant)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateAsync(Participant participant)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}