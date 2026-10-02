using System.ComponentModel.DataAnnotations;
using TourneyPlanner.Application.DTOs;
using TourneyPlanner.Application.Interfaces.Repositories;
using TourneyPlanner.Application.Interfaces.Services;
using TourneyPlanner.Domain.Entities;

namespace TourneyPlanner.Application.Services;

public class ParticipantService : IParticipantService
{
    private readonly IParticipantRepository _participantRepository;
    private readonly ITournamentRepository _tournamentRepository;

    public ParticipantService(IParticipantRepository participantRepository, ITournamentRepository tournamentRepository)
    {
        _participantRepository = participantRepository;
        _tournamentRepository = tournamentRepository;
    }

    public async Task AddParticipantAsync(int tournamentId, CreateParticipantDto dto)
    {
        var tournament = await GetDraftTournamentAsync(tournamentId);

        var existing = await _participantRepository.GetByTournamentIdAsync(tournamentId);

        if (existing.Count >= tournament.Size)
        {
            throw new ValidationException("The tournament is full");
        }
        
        var participant = new Participant
        {
            TournamentId = tournamentId,
            Name = dto.Name
        };
        
        ValidateParticipant(participant, existing);
        
        await _participantRepository.AddAsync(participant);
    }

    public async Task AddParticipantsAsync(int tournamentId, List<CreateParticipantDto> dtos)
    {
        if (dtos.Count == 0)
        {
            throw new ValidationException("At least one participant is required");
        }
        
        var tournament = await GetDraftTournamentAsync(tournamentId);
        
        var existing = await _participantRepository.GetByTournamentIdAsync(tournamentId);

        // alla nya inmatningar i batch inserten måste få plats, annars kastar metoden ett undantag och ingen läggs till.
        if (existing.Count + dtos.Count > tournament.Size)
        {
            throw new ValidationException($"The tournament only has room for {tournament.Size - existing.Count} more participants");
        }
        
        // Validerar alla deltagare innan någon sparas, antingen så att alla sparas eller ingen.
        var participants = new List<Participant>();
        foreach (var dto in dtos)
        {
            var participant = new Participant
            {
                TournamentId = tournamentId,
                Name = dto.Name
            };
            // Jämför mot både sparade deltagare och nya deltagare i listan, så att dubletter inte sparas.
            ValidateParticipant(participant, existing.Concat(participants));
            participants.Add(participant);
        }
        await _participantRepository.AddRangeAsync(participants);
    }

    public async Task<List<ParticipantDto>> GetParticipantsByTournamentIdAsync(int tournamentId)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(tournamentId);
        if (tournament == null)
        {
            // KeyNotFoundException för att visa 404 not found svar från endpointen
            throw new KeyNotFoundException("Tournament not found");
        }

        var participants = await _participantRepository.GetByTournamentIdAsync(tournamentId);
        
        // gör om varje Participant till en ParticipantDto genom våran hjälpmetod.
        return participants.Select(MapToDto).ToList();
    }

    public async Task<ParticipantDto> GetParticipantByIdAsync(int id)
    {
        var participant = await GetParticipantOrThrowAsync(id);
        return MapToDto(participant);
    }

    public async Task UpdateParticipantAsync(int id, UpdateParticipantDto dto)
    {
        // hämtar deltagaren eller kastar ett undantag om den inte finns.
        var participant = await GetParticipantOrThrowAsync(id);
        
        // kollar att turneringen finns och har statusen Draft, annars kastar metoden ett undantag.
        await GetDraftTournamentAsync(participant.TournamentId);

        // hämtar övriga deltagare i turneringen, exkluderar den som uppdateras.
        // detta ifall deltagaren vill behålla sitt gamla namn utan att den räknas som dubblett.
        var others = (await _participantRepository.GetByTournamentIdAsync(participant.TournamentId))
            .Where(p => p.Id != id);
        
        participant.Name = dto.Name;
        ValidateParticipant(participant, others);
        
        await _participantRepository.UpdateAsync(participant);
    }

    public async Task DeleteParticipantAsync(int id)
    {
        var participant = await GetParticipantOrThrowAsync(id);
        await GetDraftTournamentAsync(participant.TournamentId);
        
        await _participantRepository.DeleteAsync(participant.Id);
    }

    // Hjälpmetoder
    
    private async Task<Tournament> GetDraftTournamentAsync(int tournamentId)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(tournamentId);

        if (tournament == null)
        {
            throw new KeyNotFoundException("Tournament not found");
        }
        
        if (tournament.TournamentStatus != Tournament.Status.Draft)
        {
            // ValidationException för att visa 400 bad request svar från endpointen
            throw new ValidationException("Tournament must be in draft status");
        }

        return tournament;
    }

    private async Task<Participant> GetParticipantOrThrowAsync(int id)
    {
        // returnerar Participant eller kastar ett undantag om den inte finns.
        return await _participantRepository.GetParticipantById(id) ?? 
               throw new KeyNotFoundException("Participant not found");
    }

    private static void ValidateParticipant(Participant participant, IEnumerable<Participant> existing)
    {
        Validator.ValidateObject(participant, new ValidationContext(participant), validateAllProperties: true);

        // kollar om namnet redan finns i listan med deltagare, ignorerar skillnaden på stor och liten bokstav.
        if (existing.Any(p => p.Name.Equals(participant.Name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ValidationException("Participant already exists");
        }
    }
    
    private static ParticipantDto MapToDto(Participant participant)
    {
        return new ParticipantDto
        {
            Id = participant.Id,
            TournamentId = participant.TournamentId,
            Name = participant.Name
        };
    }
}