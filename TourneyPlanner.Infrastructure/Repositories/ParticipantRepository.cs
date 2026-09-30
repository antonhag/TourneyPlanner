using Microsoft.EntityFrameworkCore;
using TourneyPlanner.Domain.Entities;
using TourneyPlanner.Application.Interfaces.Repositories;
using TourneyPlanner.Infrastructure.Data;

namespace TourneyPlanner.Infrastructure.Repositories;

public class ParticipantRepository : IParticipantRepository
{
    private readonly TourneyPlannerDbContext _context;

    public ParticipantRepository(TourneyPlannerDbContext context)
    {
        _context = context;
    }
    
    public async Task<Participant?> GetParticipantById(int id)
    {
        return await _context.Participants.FindAsync(id);
    }

    public async Task<List<Participant>> GetByTournamentIdAsync(int tournamentId)
    {
        return await _context.Participants.Where(p => p.TournamentId == tournamentId).ToListAsync();
    }

    public async Task AddAsync(Participant participant)
    {
        _context.Participants.Add(participant);
        await _context.SaveChangesAsync();
    }

    public async Task AddRangeAsync(List<Participant> participants)
    {
        _context.Participants.AddRange(participants);
    }

    public async Task UpdateAsync(Participant participant)
    {
        _context.Participants.Update(participant);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var participant = await _context.Participants.FindAsync(id);

        if (participant != null)
        {
            _context.Participants.Remove(participant);
            await _context.SaveChangesAsync();
        }
    }
}