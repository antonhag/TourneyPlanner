using System.Net;
using System.Net.Http.Json;
using TourneyPlanner.Application.DTOs;

namespace Api.Tests;

public class TournamentEndpointsTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public TournamentEndpointsTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostTournament_ValidInput_Returns201()
    {
        // Arrange
        var dto = new CreateTournamentDto
        {
            Name = "Paddel 2026",
            StartDate = DateTime.Today.AddDays(1)
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
            StartDate = DateTime.Today.AddDays(1)
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
            StartDate = DateTime.Today.AddDays(-1)
        };

        // Act
        var response = await _client.PostAsJsonAsync("/tournaments", dto);


        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}