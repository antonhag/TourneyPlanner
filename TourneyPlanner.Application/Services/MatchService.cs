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
        await GetTournamentOrThrowsAsync(tournamentId);
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
        
        await _matchRepository.AddRangeAsync(newMatches);
        await _matchRepository.RemoveRangeAsync(oldMatches);
        
    }

    public async Task<List<MatchDto>> GetAllScheduleAsync(int tournamentId)
    {
        await GetTournamentOrThrowsAsync(tournamentId);
        var matches = await _matchRepository.GetTournamentMatchesAsync(tournamentId);
        
        return matches.Select(MapToDto).ToList();
    }

    private async Task<Tournament> GetTournamentOrThrowsAsync(int tournamentId)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(tournamentId);
        if (tournament == null)
        {
            throw new ValidationException("Tournament not found");
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

    private static MatchDto MapToDto(Match match)
    {
        return new MatchDto
        {
            Id = match.Id,
            TournamentId = match.TournamentId,
            Round = match.Round,
            HomeParticipantId = match.HomeParticipantId,
            AwayParticipantId = match.AwayParticipantId,
            HomeScore = match.HomeScore,
            AwayScore = match.AwayScore,
        };
    }
}
