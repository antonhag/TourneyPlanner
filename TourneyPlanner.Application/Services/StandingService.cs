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
        
        var participants = await _participantRepository.GetByTournamentIdAsync(tournamentId);

        var standings = new List<StandingDto>();
        
        foreach (var participant in participants)
        {
            standings.Add(new StandingDto
            {
                ParticipantId = participant.Id,
                Name = participant.Name
            });
        }

        var matches = await _matchRepository.GetTournamentMatchesAsync(tournamentId);
        
        // bara de matchar som har spelats (de som har resultat)
        var matchesPlayed = matches
            .Where(m => m.HomeScore != null && m.AwayScore != null).ToList();
        
        // raderna i tabellen med deltagare ID som nyckel, för att hitta varje lag direkt
        var rows = standings.ToDictionary(s => s.ParticipantId);

        foreach (var match in matchesPlayed)
        {
            var home = rows[match.HomeParticipantId];
            var away = rows[match.AwayParticipantId];
            
            // ! tar bort null varningen
            var homeScore = match.HomeScore!.Value;
            var awayScore = match.AwayScore!.Value;
            
            // båda lagen uppdateras med en match spelad och dess resultat
            home.Played++;
            away.Played++;
            
            home.ScoreFor += homeScore;
            home.ScoreAgainst += awayScore;
            away.ScoreFor += awayScore;
            away.ScoreAgainst += homeScore;
            
            // sätter antingen vinst, förlust eller oavgjort till lagen
            if (homeScore > awayScore)
            {
                home.Won++;
                away.Lost++;
            }
            else if (homeScore < awayScore)
            {
                home.Lost++;
                away.Won++;
            }
            else
            {
                home.Drawn++;
                away.Drawn++;
            }
        }
        
        return standings;
    }
}