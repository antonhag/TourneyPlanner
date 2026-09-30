using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using TourneyPlanner.Infrastructure.Data;
using TourneyPlanner.Application.DTOs;
using TourneyPlanner.Domain.Entities;

namespace Api.Tests;

public class ParticipantEndpointsTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ParticipantEndpointsTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    // Lägger in en turnering i testdatabasen där vi får tillbaka dess id för att kunna använda den i testerna.
    private async Task<int> CreateTournamentAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TourneyPlannerDbContext>();

        var tournament = new Tournament { Name = "Padel 2026", StartDate = DateTime.Today.AddDays(1) };
        db.Tournaments.Add(tournament);
        await db.SaveChangesAsync();
        
        return tournament.Id;
    }

    [Fact]
    public async Task PostParticipant_ValidInput_Returns201()
    {
        // Arrange
        var tournamentId = await CreateTournamentAsync();
        var dto = new CreateParticipantDto { Name = "Legenderna" };

        // Act
        var response = await _client.PostAsJsonAsync($"/tournaments/{tournamentId}/participants", dto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task PostParticipant_TournamentNotFound_Returns404()
    {
        // Arrange
        var dto = new CreateParticipantDto { Name = "Legenderna" };

        // Act
        var response = await _client.PostAsJsonAsync("/tournaments/1337/participants", dto);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PostParticipantsBatch_ThenGetThem_ShouldReturnAllParticipants()
    {
        // Arrange
        var tournamentId = await CreateTournamentAsync();
        var dtos = new List<CreateParticipantDto>
        {
            new() { Name = "Legenderna" },
            new() { Name = "Maskinerna" }
        };

        // Act
        await _client.PostAsJsonAsync($"/tournaments/{tournamentId}/participants/batch", dtos);
        
        var participants = await _client.GetFromJsonAsync<List<ParticipantDto>>($"/tournaments/{tournamentId}/participants");
        
        // Assert
        Assert.NotNull(participants); // Kollar så att den ej är null
        Assert.Equal(2, participants.Count); // Kollar att vi fick 2 deltagare
    }
}