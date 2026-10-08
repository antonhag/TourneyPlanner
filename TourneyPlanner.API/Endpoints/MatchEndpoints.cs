using System.ComponentModel.DataAnnotations;
using TourneyPlanner.Application.DTOs;
using TourneyPlanner.Application.Interfaces.Services;

namespace TourneyPlanner.API.Endpoints;

public static class MatchEndpoints
{
    public static void MapMatchEndpoints(this WebApplication app)
    {
        // Schemat hör till en turnering men ligger här så att TorunamentEndpoints lämnas orörd
        var tournamentGroup = app.MapGroup("/tournaments");
        tournamentGroup.MapPost("/{id:int}/schedule", GenerateSchedule).RequireAuthorization();
        tournamentGroup.MapGet("/{id:int}/schedule", GetSchedule);
        tournamentGroup.MapPut("/{id:int}/schedule", UpdateSchedule).RequireAuthorization();
        
        // resultat registeras på en enskild match
        var matchGroup = app.MapGroup("/matches");
        matchGroup.MapPut("/{id:int}/result", RegisterResult).RequireAuthorization();
    }

    private static async Task<IResult> GenerateSchedule(int id, IMatchService service)
    {
        try
        {
            await service.GenerateScheduleAsync(id);
            return Results.Created();
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

    private static async Task<IResult> UpdateSchedule(int id, IMatchService service)
    {
        try
        {
            await service.UpdateScheduleAsync(id);
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

    private static async Task<IResult> GetSchedule(int id, IMatchService service)
    {
        try
        {
            var result = await service.GetAllScheduleAsync(id);
            return Results.Ok(result);
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

    private static async Task<IResult> RegisterResult(int id, UpdateMatchResultDto dto, IMatchService service)
    {
        try
        {
            await service.RegisterResultAsync(id, dto);
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

