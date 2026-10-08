using System.ComponentModel.DataAnnotations;
using TourneyPlanner.Application.DTOs;
using TourneyPlanner.Application.Interfaces.Repositories;
using TourneyPlanner.Application.Interfaces.Services;
using TourneyPlanner.Domain.Entities;

namespace TourneyPlanner.Application.Services;

public class TournamentService : ITournamentService
{
    private readonly ITournamentRepository _tournamentRepository;
    private readonly IParticipantRepository _participantRepository;
    private readonly IMatchService _matchService;

    public TournamentService(ITournamentRepository tournamentRepository,
        IParticipantRepository participantRepository,
        IMatchService matchService)
    {
        _tournamentRepository = tournamentRepository;
        _participantRepository = participantRepository;
        _matchService = matchService;
    }

    public async Task CreateTournamentAsync(CreateTournamentDto dto)
    {
        var tournament = new Tournament()
        {
            Name = dto.Name,
            StartDate = dto.StartDate,
            CreatedAt = DateTime.Now,
            Size = dto.Size
        };
        
        ValidateTournament(tournament);
        
        await _tournamentRepository.AddAsync(tournament);
        
    }

    public async Task<List<TournamentDto>> GetAllTournamentsAsync()
    {
        var tournaments = await _tournamentRepository.GetAllAsync();
        return tournaments.Select(MapToDto).ToList();
    }

    public async Task<TournamentDto> GetTournamentByIdAsync(int id)
    {
        var tournament = await GetExistingTournamentAsync(id);
        return MapToDto(tournament);
    }

    public async Task UpdateTournamentAsync(int id, UpdateTournamentDto dto)
    {
        var tournament = await GetExistingTournamentAsync(id);

        if (tournament.TournamentStatus != Tournament.Status.Draft)
        {
            throw new ValidationException("Tournament status must be Draft to be edited");
        }
        
        tournament.Name = dto.Name;
        tournament.StartDate = dto.StartDate;
        tournament.Size = dto.Size;
        
        ValidateTournament(tournament);
        
        await _tournamentRepository.UpdateAsync(tournament);
    }

    public async Task DeleteTournamentAsync(int id)
    {
        var tournament = await GetExistingTournamentAsync(id);

        if (tournament.TournamentStatus ==  Tournament.Status.Active)
        {
            throw new ValidationException("An active tournament cannot be deleted");
        }
        
        await _tournamentRepository.DeleteAsync(tournament);
    }

    public async Task StartTournamentAsync(int id)
    {
        var tournament = await GetExistingTournamentAsync(id);

        if (tournament.TournamentStatus != Tournament.Status.Draft)
        {
            throw new ValidationException("Only tournaments with status = draft can be started");
        }

        var participants = await _participantRepository.GetByTournamentIdAsync(id);

        if (participants.Count < 2)
        {
            throw new ValidationException("Must have atleast 2 particiapnts to start the tournament");
        }

        await _matchService.GenerateScheduleAsync(id);
        
        tournament.TournamentStatus = Tournament.Status.Active;
        await _tournamentRepository.UpdateAsync(tournament);
    }


    private async Task<Tournament> GetExistingTournamentAsync(int id)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(id);

        if (tournament == null)
        {
            throw new KeyNotFoundException($"Tournament with id {id} was not found");
        }
        return tournament;
    }

    private static void ValidateTournament(Tournament tournament)
    {
        Validator.ValidateObject(tournament, new ValidationContext(tournament), validateAllProperties: true);
        
        if (tournament.StartDate < DateTime.Today)
        {
            throw new ValidationException("Startdate cannot be in the past");
        }
        

        if (tournament.Size < 2)
        {
            throw new ValidationException("Size has to be atleast 2");
        }
    }

    private static TournamentDto MapToDto(Tournament tournament)
    {
        return new TournamentDto()
        {
            Id = tournament.Id,
            Name = tournament.Name,
            Status = tournament.TournamentStatus.ToString(),
            StartDate = tournament.StartDate,
            CreatedAt = tournament.CreatedAt,
            Size = tournament.Size
        };
    }
}