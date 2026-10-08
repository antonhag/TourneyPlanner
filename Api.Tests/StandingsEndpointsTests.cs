using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using TourneyPlanner.Application.DTOs;
using TourneyPlanner.Domain.Entities;
using TourneyPlanner.Infrastructure.Data;

namespace Api.Tests;

public class StandingsEndpointsTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;


    public StandingsEndpointsTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    // skapar en turnering med en match som redan har spelats (där hemmalaget vinner med 2-1)
    // och returnerar turnerings-id och hemmalags-id
    private async Task<(int TournamentId, int WinnerId)> CreateTournamentWithPlayedMatchAsync()
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

        var home = new Participant
        {
            TournamentId = tournament.Id,
            Name = "Lag Legenderna"
        };
        var away = new Participant
        {
            TournamentId = tournament.Id,
            Name = "Lag Maskinerna"
        };
        db.Participants.Add(home);
        db.Participants.Add(away);
        await db.SaveChangesAsync();

        db.Matches.Add(new Match
        {
            TournamentId = tournament.Id,
            Round = 1,
            HomeParticipantId = home.Id,
            AwayParticipantId = away.Id,
            HomeScore = 2,
            AwayScore = 1
        });
        await db.SaveChangesAsync();

        return (tournament.Id, home.Id);
    }
    
    [Fact]
    private async Task GetStandings_TournamentNotFound_Returns404()
    {
        // Act
        var response = await _client.GetAsync("/tournaments/1337/standings");
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    private async Task GetStandings_WithPlayedMatch_ReturnsCorrectStandings()
    {
        // Arrange
        
        // får en tuple tillbaka från CreateTournamentWithPlayedMatchAsync
        var (tournamentId, winnerId) = await CreateTournamentWithPlayedMatchAsync();

        // Act
        var standings = await _client.GetFromJsonAsync<List<StandingDto>>($"/tournaments/{tournamentId}/standings");
        
        // Assert
        Assert.NotNull(standings); 
        
        Assert.Equal(2, standings.Count); 
        
        // kollar så att vinnaren ligger först
        Assert.Equal(winnerId, standings[0].ParticipantId); 
        
        // vinnaren ska ha 3 poäng eftersom vinst ger 3
        Assert.Equal(3, standings[0].Points); 
        
        // förloraren ska ha 0 poäng eftersom förlust ger 0
        Assert.Equal(0, standings[1].Points);
    }
}