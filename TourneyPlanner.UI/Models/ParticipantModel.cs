namespace TourneyPlanner.UI.Models;

public class ParticipantModel
{
    public int Id { get; set; }
    public int TournamentId { get; set; }
    public string Name { get; set; } = string.Empty;
}
