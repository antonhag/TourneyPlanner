using TourneyPlanner.Application.Interfaces.Repositories;
using TourneyPlanner.Domain.Entities;

namespace Application.Tests.Fakes;

public class FakeParticipantRepository : IParticipantRepository
{
    public List<Participant> Participants { get; } = new();
    
    public Task<Participant?> GetParticipantById(int id)
    {
        return Task.FromResult(Participants.FirstOrDefault(p => p.Id == id));
    }

    public Task<List<Participant>> GetByTournamentIdAsync(int tournamentId)
    {
        var result = Participants.Where(p => p.TournamentId == tournamentId).ToList();
        return Task.FromResult(result);
    }

    public Task AddAsync(Participant participant)
    {
        participant.Id = Participants.Count + 1;
        Participants.Add(participant);   
        return Task.CompletedTask;
    }

    public Task AddRangeAsync(List<Participant> participants)
    {
        foreach (var participant in participants)
        {
            participant.Id = Participants.Count + 1;
            Participants.Add(participant);  
        }     
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Participant participant)
    {
        // behövs egentligen inte (interface kräver den dock), objektet i listan är samma som det som skickas in.
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        var participant = Participants.FirstOrDefault(p => p.Id == id);

        if (participant != null)
        {
            Participants.Remove(participant);
        }
        return Task.CompletedTask;
    }
}