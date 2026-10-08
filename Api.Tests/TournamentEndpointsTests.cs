using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using TourneyPlanner.Application.DTOs;
using TourneyPlanner.Domain.Entities;
using TourneyPlanner.Infrastructure.Data;

namespace Api.Tests;

public class TournamentEndpointsTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public TournamentEndpointsTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostTournament_ValidInput_Returns201()
    {
        // Arrange
        var dto = new CreateTournamentDto
        {
            Name = "Paddel 2026",
            StartDate = DateTime.Today.AddDays(1),
           
            Size = 4
        };

        // Act
        var response = await _client.PostAsJsonAsync("/tournaments", dto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task PostTournament_InvalidInput_Returns400()
    {
        // Arrange
        var dto = new CreateTournamentDto
        {
            Name = "",
            StartDate = DateTime.Today.AddDays(1),
            
            Size = 4
        };

        // Act
        var response = await _client.PostAsJsonAsync("/tournaments", dto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostTournament_InvalidDate_Returns400()
    {
        // Arrange
        var dto = new CreateTournamentDto
        {
            Name = "Paddel 2026",
            StartDate = DateTime.Today.AddDays(-1),
           
            Size = 4
            
        };

        // Act
        var response = await _client.PostAsJsonAsync("/tournaments", dto);


        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostTournament_InvalidSize_Returns400()
    {
        // Arrange
        var dto = new CreateTournamentDto
        {
            Name = "Paddel 2026",
            StartDate = DateTime.Today.AddDays(1),
            
            Size = 1
        };

        // Act
        var response = await _client.PostAsJsonAsync("/tournaments", dto);
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task GetTournaments_Returns200()
    {
        // Act
        var response = await _client.GetAsync("/tournaments");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetTournamentById_MissingId_Returns404()
    {
        // Act
        var response = await _client.GetAsync("/tournaments/9999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PutTournament_MissingId_Returns404()
    {
        // Arrange
        var dto = new UpdateTournamentDto
        {
            Name = "Paddel 2026",
            StartDate = DateTime.Today.AddDays(1),
            
            Size = 4
        };

        // Act
        var response = await _client.PutAsJsonAsync("/tournaments/999", dto);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteTournament_MissingId_Returns404()
    {
        // Act
        var response = await _client.DeleteAsync("/tournaments/999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task StartTournament_ValidTournament_Returns204_AndStatusIsActive()
    {
        // Arrange
        var tournamentId = await CreateTournamentWithParticipantsAsync(4);

        // Act
        var response = await _client.PostAsync($"/tournaments/{tournamentId}/start", null);
        var tournament = await _client.GetFromJsonAsync<TournamentDto>($"/tournaments/{tournamentId}");
        
        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.NotNull(tournament);
        Assert.Equal("Active", tournament.Status);
    }

    [Fact]
    public async Task StartTournament_MissingId_Returns404()
    {
        //Act
        var response = await _client.PostAsync("/tournaments/9999/start", null);
        
        //Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task StartTournament_TooFewParticipants_Returns400()
    {
        // Arrange
        var tournamentId = await CreateTournamentWithParticipantsAsync(1);
        
        //Act
        var response = await _client.PostAsync($"/tournaments/{tournamentId}/start", null);
        
        //Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CompleteTournament_ValidTournament_Returns200_StatusFinished()
    {
        // Arrange
        var tournamentId = await CreateActiveTournamentAsync(allMatchesPlayed: true);
        
        // Act
        var response = await _client.PostAsync($"/tournaments/{tournamentId}/complete", null);
        var winner = await response.Content.ReadFromJsonAsync<StandingDto>();
        
        var tournamentResponse = await _client.GetFromJsonAsync<TournamentDto>($"/tournaments/{tournamentId}");
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(winner);
        Assert.Equal("Player 1", winner.Name);
        Assert.NotNull(tournamentResponse);
        Assert.Equal("Finished", tournamentResponse.Status);
    }

    [Fact]
    public async Task CompleteTournament_UnplayedMatch_Returns400()
    {
        // Arrange
        var tournamentId = await CreateActiveTournamentAsync(allMatchesPlayed: false);
        
        // Act
        var response = await _client.PostAsync($"/tournaments/{tournamentId}/complete", null);
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CompleteTournament_MissingId_Returns404()
    {
        
        // Act
        var response = await _client.PostAsync("/tournaments/99999/complete", null);
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
   
    
    //lägger in en turnering med ett vissta antal deltagare direkt i testdatabasen
    private async Task<int> CreateTournamentWithParticipantsAsync(int participantCount)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TourneyPlannerDbContext>();

        var tournament = new Tournament
        {
            Name = "Paddel 2026",
            StartDate = DateTime.Today.AddDays(1),
            Size = 4
        };
        db.Tournaments.Add(tournament);
        await db.SaveChangesAsync();

        for (var i = 1; i <= participantCount; i++)
        {
            db.Participants.Add(new Participant
            {
                TournamentId = tournament.Id,
                Name = $"Player {i}"
            });
        }
        
        await db.SaveChangesAsync();
        return tournament.Id;
    }

    private async Task<int> CreateActiveTournamentAsync(bool allMatchesPlayed)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TourneyPlannerDbContext>();

        var tournament = new Tournament
        {
            Name = "Paddel 2026",
            StartDate = DateTime.Today,
            TournamentStatus = Tournament.Status.Active,
            Size = 2
        };
        db.Tournaments.Add(tournament);
        await db.SaveChangesAsync();
        
        // skapar 2 deltagare koplade till turneringen
        var p1 = new Participant { TournamentId = tournament.Id, Name = "Player 1" };
        var p2 = new Participant { TournamentId = tournament.Id, Name = "Player 2" };
        db.Participants.AddRange(p1, p2);
        
        await db.SaveChangesAsync();// spara så deltagarna får ID

        var match = new Match
        {
            TournamentId = tournament.Id,
            HomeParticipantId = p1.Id,
            AwayParticipantId = p2.Id,
            
            HomeScore = allMatchesPlayed == true ? 3 : null,
            AwayScore = allMatchesPlayed == true ? 1 : null
        };
        db.Matches.Add(match);
        await db.SaveChangesAsync();

        return tournament.Id;
    }
}