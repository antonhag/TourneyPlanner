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
}