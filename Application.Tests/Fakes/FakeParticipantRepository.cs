using TourneyPlanner.Application.Interfaces.Repositories;
using TourneyPlanner.Domain.Entities;

namespace Application.Tests.Fakes;

public class FakeParticipantRepository : IParticipantRepository
{
    public List<Participant> Participants { get; } = new();

    public Task<Participant?> GetParticipantById(int id)
    {
        throw new NotImplementedException();
    }

    public Task<List<Participant>> GetByTournamentIdAsync(int tournamentId)
    {
        return Task.FromResult(Participants.Where(p => p.TournamentId == tournamentId).ToList());
    }

    public Task AddAsync(Participant participant)
    {
        throw new NotImplementedException();
    }
    public Task UpdateAsync(Participant participant)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}