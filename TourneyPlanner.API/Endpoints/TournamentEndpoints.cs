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
        group.MapPost("/", CreateTournament).RequireAuthorization();

        group.MapPost("/{id:int}/start", StartTournament).RequireAuthorization();
        group.MapGet("/", GetAllTournaments);
        group.MapGet("/{id:int}", GetTournamentById);
        group.MapPut("/{id:int}", UpdateTournament).RequireAuthorization();
        group.MapDelete("/{id:int}", DeleteTournament).RequireAuthorization();
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

    private static async Task<IResult> GetAllTournaments(ITournamentService service)
    {
        var tournaments = await service.GetAllTournamentsAsync();
        return Results.Ok(tournaments);
    }

    private static async Task<IResult> GetTournamentById(int id, ITournamentService service)
    {
        try
        {
            var tournament = await service.GetTournamentByIdAsync(id);
            return Results.Ok(tournament);
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(ex.Message);
        }
    }

    private static async Task<IResult> UpdateTournament(int id, UpdateTournamentDto dto, ITournamentService service)
    {
        try
        {
            await service.UpdateTournamentAsync(id, dto);
            return Results.NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(ex.Message);
        }
        catch (ValidationException ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    private static async Task<IResult> DeleteTournament(int id, ITournamentService service)
    {
        try
        {
            await service.DeleteTournamentAsync(id);
            return Results.NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(ex.Message);
        }
        catch (ValidationException ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    private static async Task<IResult> StartTournament(int id, ITournamentService service)
    {
        try
        {
            await service.StartTournamentAsync(id);
            return Results.NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(ex.Message);
        }
        catch (ValidationException ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }
}