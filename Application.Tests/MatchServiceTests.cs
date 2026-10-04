using System.ComponentModel.DataAnnotations;
using Application.Tests.Fakes;
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
        // Turneringen som alla tester använder, status är draft som standard
        _tournamentRepository.Tournaments.Add(new Tournament()
        {
            Id = TournamentId,
            Name = "Paddel 2026"
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
}




