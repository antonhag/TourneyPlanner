using System.ComponentModel.DataAnnotations;
using Application.Tests.Fakes;
using TourneyPlanner.Application.DTOs;
using TourneyPlanner.Application.Services;
using TourneyPlanner.Domain.Entities;

namespace Application.Tests;

public class MatchServiceTests
{
    private const int TournamentId = 1;

    private readonly FakeMatchRepository _matchRepository = new();
    private readonly FakeParticipantRepository _participantRepository = new();
    private readonly FakeTournamentRepository _tournamentRepository = new();
    private readonly MatchService _sut;

    public MatchServiceTests()
    {
        _sut = new MatchService(_matchRepository, _participantRepository, _tournamentRepository);

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

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public async Task GenerateSchedule_LessThanTwoParticipants_ShouldNotSave(int count)
    {
        // Arrange                                                                                                                                                                                                                
        AddParticipant(count);

        // Act & Assert                                                                                                                                                                                                           
        await Assert.ThrowsAsync<ValidationException>(() => _sut.GenerateScheduleAsync(TournamentId));
        Assert.Empty(_matchRepository.Matches);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(8)]
    public async Task GenerateSchedule_EveryPairMeetsExactlyOnce(int participants)
    {
        // Arrange
        AddParticipant(participants);

        // Act
        await _sut.GenerateScheduleAsync(TournamentId);

        // Assert
        // sorterar paret så att 1-2 och 2-1 räknas som samma match
        var pairs =  _matchRepository.Matches
            .Select(m=>(Math.Min(m.HomeParticipantId, m.AwayParticipantId),
                                Math.Max(m.HomeParticipantId, m.AwayParticipantId)))
            .ToList(); 
        
        Assert.Equal(pairs.Count, pairs.Distinct().Count());
        Assert.All(_matchRepository.Matches, m=> Assert.NotEqual(m.HomeParticipantId, m.AwayParticipantId));
    }
    [Theory]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(8)]
    public async Task GenerateSchedule_NoParticipantPlaysTwiceInTheSameRound(int participants)
    {
        // Arrange
        AddParticipant(participants);

        // Act
        await _sut.GenerateScheduleAsync(TournamentId);

        // Assert
        foreach (var round in _matchRepository.Matches.GroupBy(m => m.Round))
        {
            var playersInRound = round
                .SelectMany(m => new[] { m.HomeParticipantId, m.AwayParticipantId })
                .ToList();
            
            Assert.Equal(playersInRound.Count, playersInRound.Distinct().Count());
        }
    }

    [Theory]
    [InlineData(4, 3)] // Jämnt: n - 1 rundor
    [InlineData(5, 5)] // Udda: n rundor, alla står över en gång
    [InlineData(8, 7)]
    public async Task GenerateSchedule_CreatesCorrectNumberOfRounds(int participants, int expectedRounds)
    {
        // Arrange
        AddParticipant(participants);

        // Act
        await _sut.GenerateScheduleAsync(TournamentId);

        // Assert   
        var rounds = _matchRepository.Matches.Select(m => m.Round).Distinct().Count();
        Assert.Equal(expectedRounds, rounds);
    }

    [Fact]
    public async Task GenerateSchedule_TournamentNotFound_ShouldNotSave()
    {
        // Arrange
        AddParticipant(4);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GenerateScheduleAsync(1337));
        Assert.Empty(_matchRepository.Matches);
    }

    [Fact]
    public async Task GenerateSchedule_ScheduleAlreadyExists_ShouldNotSaveAgain()
    {
        // Arrange
        AddParticipant(4);
        await _sut.GenerateScheduleAsync(TournamentId);
        var count = _matchRepository.Matches.Count;

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _sut.GenerateScheduleAsync(TournamentId));
        Assert.Equal(count, _matchRepository.Matches.Count);
    }

    [Fact]
    public async Task GetMatchesByTournamentId_TournamentNotFound_Throws()
    {
        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetMatchesByTournamentIdAsync(1337));
    }

    [Fact]
    public async Task GetMatchById_MatchNotFound_Throws()
    {
        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetMatchByIdAsync(1337));
    }

    [Fact]
    public async Task UpdateMatchResult_ValidInput_SavesScore()
    {
        // Arrange
        AddParticipant(2);
        await _sut.GenerateScheduleAsync(TournamentId);
        var match = Assert.Single(_matchRepository.Matches);

        // Act
        await _sut.UpdateMatchResultAsync(match.Id, new UpdateMatchResultDto { HomeScore = 3, AwayScore = 1 });

        // Assert
        Assert.Equal(3, match.HomeScore);
        Assert.Equal(1, match.AwayScore);
    }

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(0, -1)]
    public async Task UpdateMatchResult_NegativeScore_DoesNotSave(int homeScore, int awayScore)
    {
        // Arrange
        AddParticipant(2);
        await _sut.GenerateScheduleAsync(TournamentId);
        var match = Assert.Single(_matchRepository.Matches);
        var dto = new UpdateMatchResultDto { HomeScore = homeScore, AwayScore = awayScore };

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _sut.UpdateMatchResultAsync(match.Id, dto));
        Assert.Null(match.HomeScore);
        Assert.Null(match.AwayScore);
    }

    [Fact]
    public async Task UpdateMatchResult_MatchNotFound_Throws()
    {
        // Arrange
        var dto = new UpdateMatchResultDto { HomeScore = 1, AwayScore = 0 };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.UpdateMatchResultAsync(1337, dto));
    }
}




