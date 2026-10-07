using Application.Tests.Fakes;
using TourneyPlanner.Application.Services;
using TourneyPlanner.Domain.Entities;

namespace Application.Tests;

public class StandingServiceTests
{
    private const int TournamentId = 1;
    
    private readonly FakeTournamentRepository _tournamentRepository = new();
    private readonly FakeParticipantRepository _participantRepository = new();
    private readonly FakeMatchRepository _matchRepository = new();
    private readonly StandingService _sut;

    public StandingServiceTests()
    {
        _sut = new StandingService(_tournamentRepository, _participantRepository, _matchRepository);
        
        _tournamentRepository.Tournaments.Add(new Tournament
        {
            Id = TournamentId,
            Name = "Padel 2026"
        });
    }
    
    // Hjälpmetod som lägger in X deltagare i fake-repot 
    private void AddParticipant(int count)
    {
        for (var i = 1; i <= count; i++)
        {
            _participantRepository.Participants.Add(new Participant()
            {
                Id = i,
                TournamentId = TournamentId,
                Name = $"Deltagare {i}"
            });
        }
    }

    private void AddMatch(int homeId, int awayId, int? homeScore, int? awayScore)
    {
        _matchRepository.Matches.Add(new Match
        {
            Id = _matchRepository.Matches.Count + 1,
            TournamentId = TournamentId,
            Round = 1,
            HomeParticipantId = homeId,
            AwayParticipantId = awayId,
            HomeScore = homeScore,
            AwayScore = awayScore
        });
    }
    
    [Fact]
    public async Task GetStandings_TournamentNotFound_ShouldThrow()
    {
        // Act and Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetStandingsAsync(999));
    }
}