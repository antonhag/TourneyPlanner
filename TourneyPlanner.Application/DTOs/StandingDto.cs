namespace TourneyPlanner.Application.DTOs;

public class StandingDto
{
    public int ParticipantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Played { get; set; }
    public int Won { get; set; }
    public int Drawn { get; set; }
    public int Lost { get; set; }
    public int ScoreFor { get; set; }
    public int ScoreAgainst { get; set; }
    public int ScoreDifference => ScoreFor - ScoreAgainst;
    public int Points => Won * 3 + Drawn;
}