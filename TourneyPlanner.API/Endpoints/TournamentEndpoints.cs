using System.ComponentModel.DataAnnotations;
using TourneyPlanner.Application.DTOs;
using TourneyPlanner.Application.Interfaces.Services;

namespace TourneyPlanner.API.Endpoints;

public static class TournamentEndpoints
{
    public static void MapTournamentEndpoints(this WebApplication app)
    {
        // Samlar alla endpoints under samma prefix
        var group = app.MapGroup("/tournaments");

        // Kopplar POST för tournaments till CreateTournament metoden
        group.MapPost("/", CreateTournament);
    }

    // Metoden som skapar en tournament och som körs vid varje POST request
    // Returnerar en IResult som anger om requesten lyckades eller inte
    private static async Task<IResult> CreateTournament(CreateTournamentDto dto, ITournamentService service)
    {
        try
        {
            await service.CreateTournamentAsync(dto);
            
            // Ifall valideringen lyckades returnerar den 201 Created
            return Results.Created(); 
        }
        catch (ValidationException ex)
        {
            // Om valideringen misslyckades returnerar den 400 Bad Request och ett felmeddelande
            return Results.BadRequest(ex.Message);
        }
    }
}