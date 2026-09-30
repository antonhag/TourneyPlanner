using System.ComponentModel.DataAnnotations;
using Application.Tests.Fakes;
using TourneyPlanner.Application.DTOs;
using TourneyPlanner.Application.Services;
using TourneyPlanner.Domain.Entities;

namespace Application.Tests;

public class ParticipantServiceTests
{
    private readonly FakeParticipantRepository _participantRepository = new();
    private readonly FakeTournamentRepository _tournamentRepository = new();
    private readonly ParticipantService _sut;
    
    // Idn för testturneringarna som skapas i konstruktorn
    // namngivna konstanter gör testerna lättare att läsa än siffror
    private const int DraftTournamentId = 1;
    private const int ActiveTournamentId = 2;
    
    public ParticipantServiceTests()
    {
        _sut = new ParticipantService(_participantRepository, _tournamentRepository);
        
        _tournamentRepository.Tournaments.Add(new Tournament 
        { Id = DraftTournamentId,
                Name = "Padel 2026",
                TournamentStatus = Tournament.Status.Draft 
        });
        _tournamentRepository.Tournaments.Add(new Tournament
        {
            Id = ActiveTournamentId,
            Name = "Fotbollscup 2026",
            TournamentStatus = Tournament.Status.Active
        });
    }

    [Fact]
    public async Task AddParticipant_ValidInput_DoesSave()
    {
        // Arrange
        var dto = new CreateParticipantDto { Name = "Legenderna" };

        // Act
        await _sut.AddParticipantAsync(DraftTournamentId, dto);

        // Assert
        var saved = Assert.Single(_participantRepository.Participants);
        Assert.Equal("Legenderna", saved.Name);
    }

    [Fact]
    public async Task AddParticipant_TournamentNotFound_DoesNotSave()
    {
        // Arrange
        var dto = new CreateParticipantDto { Name = "Legenderna" };

        // Act and assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.AddParticipantAsync(1337, dto));
        
        Assert.Empty(_participantRepository.Participants);
    }

    [Fact]
    public async Task AddParticipant_TournamentNotDraft_DoesNotSave()
    {
        // Arrange
        var dto = new CreateParticipantDto { Name = "Legenderna" };

        // Act and assert
        await Assert.ThrowsAsync<ValidationException>(() => _sut.AddParticipantAsync(ActiveTournamentId, dto));
        Assert.Empty(_participantRepository.Participants);
    }

    [Fact]
    public async Task AddParticipant_DuplicateName_DoesNotSave()
    {
        // Arrange
        await _sut.AddParticipantAsync(DraftTournamentId, new CreateParticipantDto { Name = "Legenderna" });

        // Act and assert
        await Assert.ThrowsAsync<ValidationException>(() => _sut.AddParticipantAsync(DraftTournamentId, new CreateParticipantDto { Name = "Legenderna" }));
        Assert.Single(_participantRepository.Participants);
    }
    
    
}