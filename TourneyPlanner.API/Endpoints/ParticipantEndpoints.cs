namespace TourneyPlanner.API.Endpoints;

public static class ParticipantEndpoints
{
    public static void MapParticipantEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/participants");
    }
}