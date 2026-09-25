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
            EndDate = dto.EndDate
        };
        
        Validator.ValidateObject(tournament, new ValidationContext(tournament), validateAllProperties: true);

        if (tournament.StartDate < DateTime.Today)
        {
            throw new ValidationException("Start date must be greater than today");
        }
        
        
        await _tournamentRepository.AddAsync(tournament);
        
    }
}