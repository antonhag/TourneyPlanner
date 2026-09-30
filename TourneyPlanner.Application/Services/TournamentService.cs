using System.ComponentModel.DataAnnotations;
using TourneyPlanner.Application.DTOs;
using TourneyPlanner.Application.Interfaces.Repositories;
using TourneyPlanner.Application.Interfaces.Services;
using TourneyPlanner.Domain.Entities;

namespace TourneyPlanner.Application.Services;

public class TournamentService : ITournamentService
{
    private readonly ITournamentRepository _tournamentRepository;

    public TournamentService(ITournamentRepository tournamentRepository)
    {
        _tournamentRepository = tournamentRepository;
    }

    public async Task CreateTournamentAsync(CreateTournamentDto dto)
    {
        var tournament = new Tournament()
        {
            Name = dto.Name,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
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
        tournament.EndDate = dto.EndDate;
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

        if (tournament.EndDate < tournament.StartDate)
        {
            throw new ValidationException("End date cannot be before startdate");
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
            EndDate = tournament.EndDate,
            Size = tournament.Size
        };
    }
}