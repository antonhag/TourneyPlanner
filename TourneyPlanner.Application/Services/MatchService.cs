using TourneyPlanner.Application.Interfaces.Repositories;
using TourneyPlanner.Application.Interfaces.Services;
using TourneyPlanner.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace TourneyPlanner.Application.Services;

public class MatchService : IMatchService
{
    private readonly IMatchRepository _matchRepository;
    private readonly IParticipantRepository _participantRepository;
    
    public  MatchService(IMatchRepository matchRepository, IParticipantRepository participantRepository)
    {
        _matchRepository = matchRepository;
        _participantRepository = participantRepository;
    }

    public async Task GenerateScheduleAsync(int tournamentId)
    {
        
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
}
