using System.ComponentModel.DataAnnotations;
using TourneyPlanner.Application.Interfaces.Services;

namespace TourneyPlanner.API.Endpoints;

public static class MatchEndpoints
{
    public static void MapMatchEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/matches");

        // Schemat hör till en turnering men ligger här så att TorunamentEndpoints lämnas orörd
        var tournamentGroup = app.MapGroup("/tournaments");
        tournamentGroup.MapPost("/{id}/schedule", GenerateSchedule);
        tournamentGroup.MapGet("/{id}/schedule", UpdateSchedule);
        tournamentGroup.MapPut("/{id}/schedule", GetAllScheduleAsync);
    }

    private static async Task<IResult> GenerateSchedule(int id, IMatchService service)
    {
        try
        {
            await service.GenerateScheduleAsync(id);
            return Results.Created();
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
        }
        catch (ValidationException ex)
        {
            return Results.BadRequest(ex.Message);
        }
        return Results.Ok();
    }

    private static async Task<IResult> GetAllScheduleAsync(int id, IMatchService service)
    {
        try
        {
            var result = await service.GetAllScheduleAsync(id);
            return Results.Ok(result);
        }
        catch (ValidationException ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }
}

