using TourneyPlanner.Application.DTOs;
using TourneyPlanner.Application.Interfaces.Repositories;
using TourneyPlanner.Application.Interfaces.Services;

namespace TourneyPlanner.Application.Services;

public class StandingService : IStandingService
{
    private readonly ITournamentRepository _tournamentRepository;
    private readonly IParticipantRepository _participantRepository;
    private readonly IMatchRepository _matchRepository;
    
    public StandingService(ITournamentRepository tournamentRepository, IParticipantRepository participantRepository, IMatchRepository matchRepository)
    {
        _tournamentRepository = tournamentRepository;
        _participantRepository = participantRepository;
        _matchRepository = matchRepository;
    }
    
    public async Task<List<StandingDto>> GetStandingsAsync(int tournamentId)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(tournamentId);
        
        if (tournament == null)
        {
            throw new KeyNotFoundException("Tournament not found");
        }
        
        var standings = new List<StandingDto>();

        return standings;
    }
}