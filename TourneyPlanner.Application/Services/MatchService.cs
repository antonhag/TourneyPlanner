using TourneyPlanner.Application.Interfaces.Repositories;
using TourneyPlanner.Application.Interfaces.Services;
using TourneyPlanner.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using TourneyPlanner.Application.DTOs;

namespace TourneyPlanner.Application.Services;

public class MatchService : IMatchService
{
    private readonly IMatchRepository _matchRepository;
    private readonly IParticipantRepository _participantRepository;
    private readonly ITournamentRepository  _tournamentRepository;
    
    public MatchService(IMatchRepository matchRepository, IParticipantRepository participantRepository, ITournamentRepository tournamentRepository)
    {
        _matchRepository = matchRepository;
        _participantRepository = participantRepository;
        _tournamentRepository = tournamentRepository;
    }

    public async Task GenerateScheduleAsync(int tournamentId)
    {
        var tournament = await GetTournamentOrThrowsAsync(tournamentId);

        if (tournament.TournamentStatus != Tournament.Status.Draft)
        {
            throw new ValidationException("Tournament must be in draft status");
        }

        if (await _matchRepository.ExistsForTournamentAsync(tournamentId))
        {
            throw new ValidationException("Schedule already exists");
        }
        
        var participants = await _participantRepository.GetByTournamentIdAsync(tournamentId);
        
        // Minst 2 spelare behövs för att starta 
        if (participants.Count < 2)
        {
            throw new ValidationException("At least two participants are required");
        }
        
        var ids = participants.Select(p => p.Id).ToList();
        var matches = CreateRoundRobin(tournamentId, ids);
        
        await _matchRepository.AddRangeAsync(matches);
    }

    public async Task UpdateScheduleAsync(int tournamentId)
    {
        var tournament = await GetTournamentOrThrowsAsync(tournamentId);
        
        // Schemat får bara göras om innan turneringen har startat
        if (tournament.TournamentStatus != Tournament.Status.Draft)
        {
            throw new ValidationException("Tournament must be in draft status");
        }
        
        var oldMatches = await _matchRepository.GetTournamentMatchesAsync(tournamentId);
        var participants = await _participantRepository.GetByTournamentIdAsync(tournamentId);

        if (participants.Count < 2)
        {
            throw new ValidationException("At least two participants are required");
        }
        
        var ids = participants.Select(p => p.Id).ToList();
        var newMatches = CreateRoundRobin(tournamentId, ids);
        
        await _matchRepository.RemoveRangeAsync(oldMatches);
        await _matchRepository.AddRangeAsync(newMatches);
    }

    public async Task<List<MatchDto>> GetAllScheduleAsync(int tournamentId)
    {
        await GetTournamentOrThrowsAsync(tournamentId);
        
        var participants = await _participantRepository.GetByTournamentIdAsync(tournamentId);
        
        // deltagarnas namn med ID som nyckel, krävs för att kunna visa namn i varje match
        var names = participants.ToDictionary(p => p.Id, p => p.Name);
        
        var matches = await _matchRepository.GetTournamentMatchesAsync(tournamentId);
        
        return matches.Select(match => MapToDto(match, names)).ToList();
    }

    public async Task RegisterResultAsync(int matchId, UpdateMatchResultDto dto)
    {
        var match = await _matchRepository.GetByIdAsync(matchId);
        
        if (match == null)
        {
            throw new KeyNotFoundException("Match not found");
        }
        
        var tournament = await GetTournamentOrThrowsAsync(match.TournamentId);

        if (tournament.TournamentStatus != Tournament.Status.Active)
        {
            throw new ValidationException("Tournament must be active to register results");
        }

        if (dto.HomeScore < 0 || dto.AwayScore < 0)
        {
            throw new ValidationException("Score cannot be negative");
        }
        
        match.HomeScore = dto.HomeScore;
        match.AwayScore = dto.AwayScore;
        
        await _matchRepository.UpdateAsync(match);
    }

    private async Task<Tournament> GetTournamentOrThrowsAsync(int tournamentId)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(tournamentId);
        if (tournament == null)
        {
            throw new KeyNotFoundException("Tournament not found");
        }
        return tournament;
    }

    private static List<Match> CreateRoundRobin(int tournamentId, List<int> participantIds)
    {
        // Kopierar Id:na till en lista där null får finnas 
        var ids = participantIds.Select(id => (int?)id).ToList();

        // lägger till en tom plats, den som möter den står över
        if (ids.Count % 2 != 0)
        {
            ids.Add(null);
        }
        
        // Med 4 deltagare behövs 3 rundor, eftersom varje deltagare har 3 motståndare. Antalet rundor är alltid antalet deltagare minus 1
        var rounds = ids.Count - 1;
        var matchesPerRound = ids.Count / 2;
        var matches = new List<Match>();

        for (int round = 1; round <= rounds; round++)
        {
            // Para ihop första med sista, andra med näst sista osv.
            for (int i = 0; i < matchesPerRound; i++)
            {
                var home = ids[i];
                var away = ids[ids.Count - 1 - i];

                // Matcher mot null sparas inte (bye)
                if (home != null && away != null)
                {
                    matches.Add(new Match
                    {
                        TournamentId = tournamentId,
                        Round = round,
                        HomeParticipantId = home.Value,
                        AwayParticipantId = away.Value
                    });
                }
            }

            // Rotation, första står still, sista flyttas till plats två
            var last = ids[ids.Count - 1];
            ids.RemoveAt(ids.Count - 1);
            ids.Insert(1, last);
        }

        return matches;
    }

    private static MatchDto MapToDto(Match match, Dictionary<int, string> names)
    {
        return new MatchDto
        {
            Id = match.Id,
            TournamentId = match.TournamentId,
            Round = match.Round,
            HomeParticipantId = match.HomeParticipantId,
            AwayParticipantId = match.AwayParticipantId,
            HomeParticipantName = names[match.HomeParticipantId],
            AwayParticipantName = names[match.AwayParticipantId],
            HomeScore = match.HomeScore,
            AwayScore = match.AwayScore,
        };
    }
}
