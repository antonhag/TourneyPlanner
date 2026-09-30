using System.ComponentModel.DataAnnotations;
using TourneyPlanner.Application.Interfaces.Services;

namespace TourneyPlanner.API.Endpoints;

public static class MatchEndpoints
{
    public static void MapMatchEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/matches");
        
        // Schemat hör till en turnering men ligger här så att TorunamentEndpoints lämnas orörd
        var tournametGroup = app.MapGroup("/tournamets");
        tournametGroup.MapPost("/{id}/schedule", GenerateSchedule);
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
}