namespace TourneyPlanner.Application.DTOs;

public class ParticipantDto
{
    public int Id { get; set; }
    public int TournamentId { get; set; }
    public string Name { get; set; } = string.Empty;
}