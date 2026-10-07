using TourneyPlanner.Application.Interfaces.Services;

namespace TourneyPlanner.API.Endpoints;

public static class StandingsEndpoints
{
    public static void MapStandingsEndpoints(this WebApplication app)
    {
        // tabellen hör till en turnering och är public, alltså krävs ingen inloggning
        var group = app.MapGroup("/tournaments/{id:int}/standings");

        group.MapGet("/", GetStandings);
    }

    private static async Task<IResult> GetStandings(int id, IStandingService service)
    {
        try
        {
            var standings = await service.GetStandingsAsync(id);
            return Results.Ok(standings);
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(ex.Message);
        }
    }
}