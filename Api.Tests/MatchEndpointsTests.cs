using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using TourneyPlanner.Application.DTOs;
using TourneyPlanner.Domain.Entities;
using TourneyPlanner.Infrastructure.Data;

namespace Api.Tests;

public class MatchEndpointsTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public MatchEndpointsTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    // skapar en turnering med status Draft med fyra deltagare direkt i test DB och returnerar dess Id
    // som vi sedan använder i testerna
    private async Task<int> CreateTournamentWithParticipantsAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TourneyPlannerDbContext>();

        var tournament = new Tournament
        {
            Name = "Padel 2026",
            StartDate = DateTime.Today.AddDays(1),
            Size = 4
        };
        db.Tournaments.Add(tournament);
        await db.SaveChangesAsync();

        for (int i = 1; i <= 4; i++)
        {
            db.Participants.Add(new Participant { Name = $"Lag {i}", TournamentId = tournament.Id });
        }
        await db.SaveChangesAsync();
        
        return tournament.Id;
    }

    [Fact]
    public async Task GenerateSchedule_ThenGet_ReturnsSixMatches()
    {
        // Arrange
        var tournamentId = await CreateTournamentWithParticipantsAsync();

        // Act - skapar schemat och sedan hämtar vi matcherna
        var response = await _client.PostAsync($"/tournaments/{tournamentId}/schedule", null);
        var matches = await _client.GetFromJsonAsync<List<MatchDto>>($"/tournaments/{tournamentId}/schedule");
        
        // Assert - jämför Http responsen, kollar ifall vi fick 6 matcher tillbaka
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(matches);
        Assert.Equal(6, matches.Count);
    }

    [Fact]
    public async Task GetSchedule_TournamentNotFound_ShouldReturn404()
    {
        // Act
        var response = await _client.GetAsync("/tournaments/1337/schedule");
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RegisterResult_ValidInput_ShouldReturn204AndSaves()
    {
        // Arrange
        var tournamentId = await CreateTournamentWithParticipantsAsync();
        await _client.PostAsync($"/tournaments/{tournamentId}/schedule", null);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<TourneyPlannerDbContext>();
            var tournament = await db.Tournaments.FindAsync(tournamentId);
            
            tournament!.TournamentStatus = Tournament.Status.Active;
            await db.SaveChangesAsync();
        }
        
        var matches = await 
            _client.GetFromJsonAsync<List<MatchDto>>($"/tournaments/{tournamentId}/schedule");

        var matchId = matches![0].Id;

        // Act
        var response = await _client.PutAsJsonAsync
        ($"/matches/{matchId}/result", new UpdateMatchResultDto
        {
            HomeScore = 2,
            AwayScore = 1
        });
        
        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var updated = await _client.GetFromJsonAsync<List<MatchDto>>
            ($"/tournaments/{tournamentId}/schedule");
        
        var match = updated!.Single(m => m.Id == matchId);
        Assert.Equal(2, match.HomeScore);
        Assert.Equal(1, match.AwayScore);
    }

    [Fact]
    public async Task RegisterResult_MatchNotFound_ShouldReturn404()
    {
        // Act
        var response = await _client.PutAsJsonAsync("/matches/1337/result",
            new UpdateMatchResultDto
            {
                HomeScore = 2,
                AwayScore = 1
            });

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}