using System.ComponentModel.DataAnnotations;
using TourneyPlanner.Application.DTOs;
using TourneyPlanner.Application.Interfaces.Services;

namespace TourneyPlanner.API.Endpoints;

public static class ParticipantEndpoints
{
    public static void MapParticipantEndpoints(this WebApplication app)
    {
        // deltagare i en viss turnering
        var tournamentGroup = app.MapGroup("/tournaments/{tournamentId:int}/participants");
        tournamentGroup.MapPost("/", AddParticipant);
        tournamentGroup.MapPost("/batch", AddParticipants);
        tournamentGroup.MapGet("/", GetParticipantsByTournamentId);
        
        // en viss deltagare
        var participantGroup = app.MapGroup("/participants");
        participantGroup.MapGet("/{id:int}", GetParticipantById);
        participantGroup.MapPut("/{id:int}", UpdateParticipant);
        participantGroup.MapDelete("/{id:int}", DeleteParticipant);
    }

    private static async Task<IResult> AddParticipant(int tournamentId, CreateParticipantDto dto, IParticipantService service)
    {
        try
        {
            await service.AddParticipantAsync(tournamentId, dto);
            return Results.Created();
        }
        // en catch ifall turneringen inte finns
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(ex.Message);
        }
        // en catch ifall valideringen misslyckades
        catch (ValidationException ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    private static async Task<IResult> AddParticipants(int tournamentId, List<CreateParticipantDto> dtos, IParticipantService service)
    {
        try
        {
            await service.AddParticipantsAsync(tournamentId, dtos);
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

    private static async Task<IResult> GetParticipantsByTournamentId(int tournamentId, IParticipantService service)
    {
        try
        {
            var participants = await service.GetParticipantsByTournamentIdAsync(tournamentId);
            return Results.Ok(participants);
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(ex.Message);
        }
    }

    private static async Task<IResult> GetParticipantById(int id, IParticipantService service)
    {
        try
        {
            var participant = await service.GetParticipantByIdAsync(id);
            return Results.Ok(participant);
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(ex.Message);
        }
    }

    private static async Task<IResult> UpdateParticipant(int id, UpdateParticipantDto dto, IParticipantService service)
    {
        try
        {
            await service.UpdateParticipantAsync(id, dto);
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

    private static async Task<IResult> DeleteParticipant(int id, IParticipantService service)
    {
        try
        {
            await service.DeleteParticipantAsync(id);
            // NoContent betyder att anropet lyckades men att den har ingenting att skicka tillbaka eftersom det endast är en DELETE request
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