using System.ComponentModel.DataAnnotations;
using TourneyPlanner.Application.DTOs;
using TourneyPlanner.Application.Interfaces.Services;

namespace TourneyPlanner.API.Endpoints;

public static class MatchEndpoints
{
    public static void MapMatchEndpoints(this WebApplication app)
    {
        // schema och matcher i en viss turnering
        // Schemat hör till en turnering men ligger här så att TournamentEndpoints lämnas orörd
        var tournamentGroup = app.MapGroup("/tournaments/{tournamentId:int}");
        tournamentGroup.MapPost("/schedule", GenerateSchedule);
        tournamentGroup.MapGet("/matches", GetMatchesByTournamentId);

        // en viss match
        var matchGroup = app.MapGroup("/matches");
        matchGroup.MapGet("/{id:int}", GetMatchById);
        matchGroup.MapPut("/{id:int}/result", UpdateMatchResult);
    }

    private static async Task<IResult> GenerateSchedule(int tournamentId, IMatchService service)
    {
        try
        {
            await service.GenerateScheduleAsync(tournamentId);
            return Results.Created();
        }
        // en catch ifall turneringen inte finns
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(ex.Message);
        }
        // en catch ifall det är för få deltagare eller schemat redan finns
        catch (ValidationException ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    private static async Task<IResult> GetMatchesByTournamentId(int tournamentId, IMatchService service)
    {
        try
        {
            var matches = await service.GetMatchesByTournamentIdAsync(tournamentId);
            return Results.Ok(matches);
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(ex.Message);
        }
    }

    private static async Task<IResult> GetMatchById(int id, IMatchService service)
    {
        try
        {
            var match = await service.GetMatchByIdAsync(id);
            return Results.Ok(match);
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(ex.Message);
        }
    }

    private static async Task<IResult> UpdateMatchResult(int id, UpdateMatchResultDto dto, IMatchService service)
    {
        try
        {
            await service.UpdateMatchResultAsync(id, dto);
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