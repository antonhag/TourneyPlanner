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
    private readonly TournamentService _sut;
    
    public TournamentServiceTests()
    {
        _sut = new TournamentService(_repository);
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
}