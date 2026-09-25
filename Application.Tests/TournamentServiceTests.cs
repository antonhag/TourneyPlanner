using Application.Tests.TestData;
using TourneyPlanner.Application.DTOs;
using TourneyPlanner.Application.Interfaces.Repositories;
using TourneyPlanner.Application.Interfaces.Services;
using TourneyPlanner.Application.Services;
using TourneyPlanner.Domain.Entities;
using TourneyPlanner.Infrastructure.Repositories;

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
        };
        // Act
        await _sut.CreateTournamentAsync(dto);

        // Assert
        Assert.Equal(Tournament.Status.Draft, _repository.Tournaments[0].TournamentStatus);
    }

    [Theory]
    [MemberData(nameof(CreateTournamentTestData.Valid), MemberType = typeof(CreateTournamentTestData))]
    
    public async Task CreateTournament_ValidInput_CreatesTournament(CreateTournamentDto dto)
    {
        // Act
        await _sut.CreateTournamentAsync(dto);
        
        // Assert
        Assert.Single(_repository.Tournaments);
    }
    
    [Theory]
    [MemberData(nameof(CreateTournamentTestData.Invalid), MemberType = typeof(CreateTournamentTestData))]
    public async Task CreateTournament_InvalidInput_DoesNotCreateTournament(CreateTournamentDto dto)
    {
        // Act
        await _sut.CreateTournamentAsync(dto);
        
        // Assert
        Assert.Single(_repository.Tournaments);
    }
}