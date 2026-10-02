namespace TourneyPlanner.UI.Models;

public class TournamentModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Status  { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Size { get; set; }
}