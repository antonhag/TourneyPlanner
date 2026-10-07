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

    [Fact]
    public async Task GetStandings_NoMatchesPlayed_AllParticipantsWithZeroPoints()
    {
        // Arrange
        AddParticipant(3);

        // Act
        var standings = await _sut.GetStandingsAsync(TournamentId);

        // Assert
        Assert.Equal(3, standings.Count); // kollar så att tabellen har exakt 3 rader
        
        // kollar så att alla deltagare i tabellen har 0 poäng eftesom inga matcher har spelats
        Assert.All(standings, s => Assert.Equal(0, s.Points)); 
    }

    [Fact]
    public async Task GetStandings_Win_ShouldGiveThreePointsToWinner()
    {
        // Arrange
        AddParticipant(2);
        
        // skapar en match där participant med id 1 vann
        AddMatch(1, 2, 2, 1);

        // Act
        var standings = await _sut.GetStandingsAsync(TournamentId);

        // Assert
        var winner = standings.Single(s => s.ParticipantId == 1);
        var loser = standings.Single(s => s.ParticipantId == 2);
        Assert.Equal(3, winner.Points); // kollar ifall vinnaren fick 3 poäng
        Assert.Equal(1, winner.Won); // kollar ifall vinnaren har en match vunnen
        Assert.Equal(0, loser.Points);  // kollar ifall förloraren har 0 poäng
        Assert.Equal(1, loser.Lost); // kollar ifall förloraren en match förlorad
    }

    [Fact]
    public async Task GetStandings_Draw_ShouldGiveOnePointEach()
    {
        // Arrange
        AddParticipant(2);
        
        // skapar en match där det blir lika (1-1)
        AddMatch(1, 2, 1, 1);

        // Act
        var standings = await _sut.GetStandingsAsync(TournamentId);

        // Assert
        Assert.All(standings, s => Assert.Equal(1, s.Points));
        Assert.All(standings, s => Assert.Equal(1, s.Drawn));
    }

    [Fact]
    public async Task GetStandings_SortsByPoints()
    {
        // Arrange
        AddParticipant(3);
        AddMatch(3, 1, 1, 0);
        AddMatch(3, 2, 1, 0);

        // Act
        var standings = await _sut.GetStandingsAsync(TournamentId);

        // Assert - deltagare 3 har flest poäng och ska ligga först i tabellen
        Assert.Equal(3, standings[0].ParticipantId);
        Assert.Equal(6, standings[0].Points);
    }

    [Fact]
    public async Task GetStandings_EqualPoints_SortsByScoreDifference()
    {
        // Arrange
        AddParticipant(4);  
        AddMatch(3, 1, 1, 0);
        AddMatch(3, 2, 1, 0);
        AddMatch(4, 2, 2, 0);
        AddMatch(4, 1, 3, 0);

        // Act
        var standings = await _sut.GetStandingsAsync(TournamentId);
        
        // Assert - kollar så att deltagare 4 ska ligga före 3
        Assert.Equal(4, standings[0].ParticipantId);
        Assert.Equal(3, standings[1].ParticipantId);
    }
}