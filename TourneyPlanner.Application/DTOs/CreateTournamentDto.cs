namespace TourneyPlanner.Application.DTOs;

public class CreateTournamentDto
{
    public string Name { get; set; } = string.Empty;
   
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}