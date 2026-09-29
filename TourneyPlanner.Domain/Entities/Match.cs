namespace TourneyPlanner.Domain.Entities;

public class Match
{
    public int Id { get; set; }
    public int TournamentId { get; set; }
    public int Round { get; set; }
    
    public int HomeParticipantId { get; set; }
    public int AwayParticipantId { get; set; }
    
    public int? HomeScore { get; set; }
    public int? AwayScore { get; set; }
}