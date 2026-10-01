using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using TourneyPlanner.Infrastructure.Data;
using TourneyPlanner.Application.DTOs;
using TourneyPlanner.Domain.Entities;

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

    // Lägger in en turnering med X deltagare i testdatabasen och returnerar turneringens id.
    private async Task<int> CreateTournamentWithParticipantsAsync(int count)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TourneyPlannerDbContext>();

        var tournament = new Tournament { Name = "Padel 2026", StartDate = DateTime.Today.AddDays(1) };
        db.Tournaments.Add(tournament);
        await db.SaveChangesAsync();

        for (var i = 1; i <= count; i++)
        {
            db.Participants.Add(new Participant { TournamentId = tournament.Id, Name = $"Deltagare {i}" });
        }
        await db.SaveChangesAsync();

        return tournament.Id;
    }

    [Fact]
    public async Task PostSchedule_ValidInput_Returns201()
    {
        // Arrange
        var tournamentId = await CreateTournamentWithParticipantsAsync(4);

        // Act
        var response = await _client.PostAsync($"/tournaments/{tournamentId}/schedule", null);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task PostSchedule_TournamentNotFound_Returns404()
    {
        // Act
        var response = await _client.PostAsync("/tournaments/1337/schedule", null);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PostSchedule_TooFewParticipants_Returns400()
    {
        // Arrange
        var tournamentId = await CreateTournamentWithParticipantsAsync(1);

        // Act
        var response = await _client.PostAsync($"/tournaments/{tournamentId}/schedule", null);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostSchedule_Twice_Returns400()
    {
        // Arrange
        var tournamentId = await CreateTournamentWithParticipantsAsync(4);
        await _client.PostAsync($"/tournaments/{tournamentId}/schedule", null);

        // Act
        var response = await _client.PostAsync($"/tournaments/{tournamentId}/schedule", null);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostSchedule_ThenGetMatches_ShouldReturnAllMatches()
    {
        // Arrange
        var tournamentId = await CreateTournamentWithParticipantsAsync(4);

        // Act
        await _client.PostAsync($"/tournaments/{tournamentId}/schedule", null);

        var matches = await _client.GetFromJsonAsync<List<MatchDto>>($"/tournaments/{tournamentId}/matches");

        // Assert
        Assert.NotNull(matches);
        Assert.Equal(6, matches.Count); // 4 deltagare ger 4 * 3 / 2 = 6 matcher
    }

    [Fact]
    public async Task GetMatches_TournamentNotFound_Returns404()
    {
        // Act
        var response = await _client.GetAsync("/tournaments/1337/matches");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetMatchById_MatchNotFound_Returns404()
    {
        // Act
        var response = await _client.GetAsync("/matches/1337");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PutResult_ValidInput_SavesScore()
    {
        // Arrange
        var tournamentId = await CreateTournamentWithParticipantsAsync(2);
        await _client.PostAsync($"/tournaments/{tournamentId}/schedule", null);
        var matches = await _client.GetFromJsonAsync<List<MatchDto>>($"/tournaments/{tournamentId}/matches");
        var matchId = Assert.Single(matches!).Id;
        var dto = new UpdateMatchResultDto { HomeScore = 3, AwayScore = 1 };

        // Act
        var response = await _client.PutAsJsonAsync($"/matches/{matchId}/result", dto);
        var match = await _client.GetFromJsonAsync<MatchDto>($"/matches/{matchId}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(3, match!.HomeScore);
        Assert.Equal(1, match.AwayScore);
    }

    [Fact]
    public async Task PutResult_NegativeScore_Returns400()
    {
        // Arrange
        var tournamentId = await CreateTournamentWithParticipantsAsync(2);
        await _client.PostAsync($"/tournaments/{tournamentId}/schedule", null);
        var matches = await _client.GetFromJsonAsync<List<MatchDto>>($"/tournaments/{tournamentId}/matches");
        var matchId = Assert.Single(matches!).Id;
        var dto = new UpdateMatchResultDto { HomeScore = -1, AwayScore = 0 };

        // Act
        var response = await _client.PutAsJsonAsync($"/matches/{matchId}/result", dto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PutResult_MatchNotFound_Returns404()
    {
        // Arrange
        var dto = new UpdateMatchResultDto { HomeScore = 1, AwayScore = 0 };

        // Act
        var response = await _client.PutAsJsonAsync("/matches/1337/result", dto);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
