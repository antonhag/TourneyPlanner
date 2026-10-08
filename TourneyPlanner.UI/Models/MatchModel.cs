namespace TourneyPlanner.UI.Models;

public class MatchModel
{
    public int Id { get; set; }
    public int Round { get; set; }
    public int HomeParticipantId { get; set; }
    public int AwayParticipantId { get; set; }
    public string HomeParticipantName { get; set; } = string.Empty;
    public string AwayParticipantName { get; set; } = string.Empty;
    public int? HomeScore { get; set; }
    public int? AwayScore { get; set; }
}