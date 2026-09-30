namespace TourneyPlanner.Application.DTOs;

public class UpdateTournamentDto
{
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Size { get; set; }
}