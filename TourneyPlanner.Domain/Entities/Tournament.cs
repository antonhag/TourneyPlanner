using System.ComponentModel.DataAnnotations;

namespace TourneyPlanner.Domain.Entities;

public class Tournament
{
    public int Id { get; set; }
    [Required]
    [MaxLength(15)]
    public string Name { get; set; } = string.Empty;
    public Status TournamentStatus { get; set; } = Status.Draft;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public List<Participant>? Participants { get; set; } = new();

    public enum Status
    {
        Draft,
        Active,
        Finished
    }
}