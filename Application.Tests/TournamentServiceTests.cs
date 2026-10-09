using System.ComponentModel.DataAnnotations;
using Application.Tests.Fakes;
using Application.Tests.TestData;
using TourneyPlanner.Application.DTOs;
using TourneyPlanner.Application.Services;
using TourneyPlanner.Domain.Entities;

namespace Application.Tests;

public class TournamentServiceTests
{
    private readonly FakeTournamentRepository _repository = new();
    private readonly FakeParticipantRepository _participantRepository = new();
    private readonly FakeMatchRepository _matchRepository = new();
    private readonly TournamentService _sut;
    
    public TournamentServiceTests()
    {
        var matchService = new MatchService(_matchRepository, _participantRepository, _repository);
        var standingService = new StandingService(_repository, _participantRepository, _matchRepository);
        
        _sut = new TournamentService(_repository, _participantRepository, matchService, standingService);
    }
    
    [Fact]
    public async Task CreateTournament_ValidInput_SavesWithCorrectName()
    {
        // Arrange
        var dto = new CreateTournamentDto()
        {
            Name = "Paddel 2026",
            StartDate = DateTime.Today.AddDays(1),
            
            Size = 4
        };

        // Act
        await _sut.CreateTournamentAsync(dto);

        // Assert
        Assert.Equal("Paddel 2026", _repository.Tournaments[0].Name);
    }

    [Fact]
    public async Task CreateTournament_ValidInput_StatusIsDraft()
    {
        // Arrange
        var dto = new CreateTournamentDto
        {
            Name = "Paddel 2026",
            StartDate = DateTime.Today.AddDays(1),
            
            Size = 4
        };
        // Act
        await _sut.CreateTournamentAsync(dto);

        // Assert
        Assert.Equal(Tournament.Status.Draft, _repository.Tournaments[0].TournamentStatus);
    }

    [Theory]
    [MemberData(nameof(CreateTournamentTestData.Valid), MemberType = typeof(CreateTournamentTestData))]
    public async Task CreateTournament_ValidInput_DoesSave(CreateTournamentDto dto)
    {
        // Act
        await _sut.CreateTournamentAsync(dto);
        
        // Assert
        Assert.Single(_repository.Tournaments);
    }

    [Theory]
    [MemberData(nameof(CreateTournamentTestData.Invalid), MemberType = typeof(CreateTournamentTestData))]
    public async Task CreateTournament_InvalidInput_DoesNotSave(CreateTournamentDto dto)
    {
        // Act & Assert
        
        // ThrowsAsync kör metoden och kollar att den kastar ValidationException.
        await Assert.ThrowsAsync<ValidationException>(() => _sut.CreateTournamentAsync(dto)); 
        
        // kollar så att listan i fake-repot är tom, alltså att valideringarna i metoden fungerade och inget sparades.
        Assert.Empty(_repository.Tournaments); 
    }
    
    [Fact]
    public async Task GetTournamentById_ExistingId_ReturnsCorrectDto()
    {
        // Arrange
        _repository.Tournaments.Add(new Tournament
        {
            Id = 1,
            Name = "Paddel 2026",
            StartDate = DateTime.Today.AddDays(1),
            EndDate = DateTime.Today.AddDays(2),
            Size = 4
        });

        // Act
        var result = await _sut.GetTournamentByIdAsync(1);

        // Assert
        Assert.Equal(1, result.Id);
        Assert.Equal("Paddel 2026", result.Name);
        Assert.Equal("Draft", result.Status);
        Assert.Equal(4, result.Size);
    }

    [Fact]
    public async Task GetTournamentById_MissingId_ThrowsKeyNotFoundException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetTournamentByIdAsync(999));
    }

    [Fact]
    public async Task GetAllTournaments_WithSavedTournaments_ReturnsAll()
    {
        // Arrange
        _repository.Tournaments.Add(new Tournament { Id = 1, Name = "Cup A", StartDate = DateTime.Today.AddDays(1), EndDate = DateTime.Today.AddDays(2), Size = 4 });
        _repository.Tournaments.Add(new Tournament { Id = 2, Name = "Cup B", StartDate = DateTime.Today.AddDays(1), EndDate = DateTime.Today.AddDays(2), Size = 8 });

        // Act
        var result = await _sut.GetAllTournamentsAsync();

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetAllTournaments_NoTournaments_ReturnsEmptyList()
    {
        // Act
        var result = await _sut.GetAllTournamentsAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task DeleteTournament_DraftStatus_RemovesTournament()
    {
        // Arrange
        _repository.Tournaments.Add(new Tournament { Id = 1, Name = "Cup A", StartDate = DateTime.Today.AddDays(1), EndDate = DateTime.Today.AddDays(2), Size = 4 });

        // Act
        await _sut.DeleteTournamentAsync(1);

        // Assert
        Assert.Empty(_repository.Tournaments);
    }

    [Fact]
    public async Task DeleteTournament_ActiveStatus_ThrowsAndKeepsTournament()
    {
        // Arrange
        _repository.Tournaments.Add(new Tournament
        {
            Id = 1,
            Name = "Cup A",
            TournamentStatus = Tournament.Status.Active,
            StartDate = DateTime.Today.AddDays(1),
            EndDate = DateTime.Today.AddDays(2),
            Size = 4
        });

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _sut.DeleteTournamentAsync(1));
        Assert.Single(_repository.Tournaments);
    }

    [Fact]
    public async Task DeleteTournament_MissingId_ThrowsKeyNotFoundException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.DeleteTournamentAsync(999));
    }
    
    [Fact]
    public async Task UpdateTournament_ValidInput_ChangesFields()
    {
        // Arrange
        _repository.Tournaments.Add(new Tournament { Id = 1, Name = "Cup A", StartDate = DateTime.Today.AddDays(1), EndDate = DateTime.Today.AddDays(2), Size = 4 });

        var dto = new UpdateTournamentDto
        {
            Name = "Cup B",
            StartDate = DateTime.Today.AddDays(3),
           
            Size = 8
        };

        // Act
        await _sut.UpdateTournamentAsync(1, dto);

        // Assert
        var updated = _repository.Tournaments.Single();
        Assert.Equal("Cup B", updated.Name);
        Assert.Equal(DateTime.Today.AddDays(3), updated.StartDate);
       
        Assert.Equal(8, updated.Size);
    }

    [Theory]
    [InlineData(Tournament.Status.Active)]
    [InlineData(Tournament.Status.Finished)]
    public async Task UpdateTournament_NotDraft_ThrowsValidationException(Tournament.Status status)
    {
        // Arrange
        _repository.Tournaments.Add(new Tournament { Id = 1, Name = "Cup A", TournamentStatus = status, StartDate = DateTime.Today.AddDays(1), EndDate = DateTime.Today.AddDays(2), Size = 4 });

        var dto = new UpdateTournamentDto { Name = "Cup B", StartDate = DateTime.Today.AddDays(1),  Size = 4 };

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _sut.UpdateTournamentAsync(1, dto));
        Assert.Equal("Cup A", _repository.Tournaments.Single().Name);
    }

    [Fact]
    public async Task UpdateTournament_MissingId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var dto = new UpdateTournamentDto { Name = "Cup B", StartDate = DateTime.Today.AddDays(1),  Size = 4 };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.UpdateTournamentAsync(999, dto));
    }

    [Fact]
    public async Task UpdateTournament_InvalidSize_ThrowsValidationException()
    {
        // Arrange
        _repository.Tournaments.Add(new Tournament { Id = 1, Name = "Cup A", StartDate = DateTime.Today.AddDays(1), EndDate = DateTime.Today.AddDays(2), Size = 4 });

        var dto = new UpdateTournamentDto { Name = "Cup A", StartDate = DateTime.Today.AddDays(1),  Size = 1 };

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _sut.UpdateTournamentAsync(1, dto));
    }

    [Fact]
    public async Task CreateTournament_SetsCreatedAtDate()
    {
        // Arrange
        var dto = new CreateTournamentDto {Name = "TestCreateDate", StartDate = DateTime.Today.AddDays(1), Size = 4};
        var before = DateTime.Now;
        
        // Act
        await _sut.CreateTournamentAsync(dto);
        
        // Assert
        var saved = Assert.Single(_repository.Tournaments);
        
        // Ifall vår actual är mellan before och Datetime.now så funkar testet som det ska
        Assert.InRange(saved.CreatedAt, before, DateTime.Now);
    }

    [Fact]
    public async Task StartTournament_ValidTounrament_BecomesActiveAndCreatesMatches()
    {
        // Arrange
        AddTournament();
        AddParticipants(4);
        
        // Act
        await _sut.StartTournamentAsync(1);
        
        // Assert
        Assert.Equal(Tournament.Status.Active, _repository.Tournaments.Single().TournamentStatus);
        Assert.Equal(6, _matchRepository.Matches.Count);
    }

    [Fact]
    public async Task StartTournament_MissingId_ThrowsKeyNotFoundException()
    {
        //Act and assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.StartTournamentAsync(999));
    }

    [Theory]
    [InlineData(Tournament.Status.Active)]
    [InlineData(Tournament.Status.Finished)]
    public async Task StartTournament_NotDraft_ThrowsAndKeepsStatus(Tournament.Status status)
    {
        // Arrange
        AddTournament(status);
        AddParticipants(4);
        
        // Act
        // Assert
        await Assert.ThrowsAsync<ValidationException>(() => _sut.StartTournamentAsync(1));
        Assert.Equal(status, _repository.Tournaments.Single().TournamentStatus);
        Assert.Empty(_matchRepository.Matches);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public async Task StartTournament_TooFewParticipants_ThrowsAndStaysDraft(int participantCount)
    {
        // Arrange
        AddTournament();
        AddParticipants(participantCount);
        
        // Act
        // Assert
        await Assert.ThrowsAsync<ValidationException>(() => _sut.StartTournamentAsync(1));
        Assert.Equal(Tournament.Status.Draft, _repository.Tournaments.Single().TournamentStatus);
        Assert.Empty(_matchRepository.Matches);
    }

    [Fact]
    public async Task CompleteTournament_ValidTournament_ReturnsWinnerAndFinishes()
    {
        // Arrange
        AddTournament(Tournament.Status.Active);
        AddParticipants(2);
        AddMatch(1, 2, 3, 1); //deltagar 1 vinner
        
        // Act
        var winner = await _sut.CompleteTournamentAsync(1);

        // Assert
        var tournament = _repository.Tournaments.Single();
        Assert.Equal(1, winner.ParticipantId);
        Assert.Equal(Tournament.Status.Finished, tournament.TournamentStatus);
        Assert.NotNull(tournament.EndDate);
    }

    [Theory]
    [InlineData(Tournament.Status.Draft)]
    [InlineData(Tournament.Status.Finished)]
    public async Task CompleteTournament_WhenInactive_ThrowsAndStaysSameStatus(Tournament.Status status)
    {
        // Arrange
        AddTournament(status);
        AddParticipants(2);
        AddMatch(1, 2, 3, 1); //deltagare 1 vinner
        
        // Act
        // Assert
        await Assert.ThrowsAsync<ValidationException>(() => _sut.CompleteTournamentAsync(1));
        var tournament = _repository.Tournaments.Single();
        Assert.Equal(status, tournament.TournamentStatus);
        
    }

    [Fact]
    public async Task CompleteTournament_UnplayedMatch_ThrowsExceptionAndDoesNotFinish()
    {
        // Arrange
        AddTournament(Tournament.Status.Active);
        AddParticipants(2);
        AddMatch(1, 2, null, null);
        
        // Act
        // Assert
        await Assert.ThrowsAsync<ValidationException>(() => _sut.CompleteTournamentAsync(1));
        
        var tournament = _repository.Tournaments.Single();
        Assert.Equal(Tournament.Status.Active, tournament.TournamentStatus);
        Assert.Null(tournament.EndDate);
    }

    [Fact]
    public async Task CompleteTournament_TournamentDoesntExist_ThrowsKeyNotFoundException()
    {
        // Arrange
        const int nonExistingId = 999;
        
        // Act
        // Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.CompleteTournamentAsync(nonExistingId));
    }
    
    // hjälpmetoder
    private void AddTournament(Tournament.Status status = Tournament.Status.Draft)
    {
        _repository.Tournaments.Add(new Tournament
        {
            Id = 1,
            Name = "Paddel 2026",
            TournamentStatus = status,
            StartDate = DateTime.Today.AddDays(1),
            EndDate = null, //DateTime.Today.AddDays(2),
            Size = 4
        });
    }

    private void AddParticipants(int count)
    {
        for (var i = 1; i <= count; i++)
        {
            _participantRepository.Participants.Add(new Participant
            {
                Id = i,
                TournamentId = 1,
                Name = $"Player {i}"
            });
        }
    }

    private void AddMatch(int homeId, int awayId, int? homeScore, int? awayScore)
    {
        _matchRepository.Matches.Add(new Match
        {
            Id = _matchRepository.Matches.Count + 1,
            TournamentId = 1,
            Round = 1,
            HomeParticipantId = homeId,
            AwayParticipantId = awayId,
            HomeScore = homeScore,
            AwayScore = awayScore
        });
    }
}