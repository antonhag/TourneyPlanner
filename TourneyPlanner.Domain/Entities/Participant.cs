using System.ComponentModel.DataAnnotations;

namespace TourneyPlanner.Domain.Entities;

public class Participant
{
    public int Id { get; set; }
    public int TournamentId { get; set; }
    [Required]
    [MaxLength(25)]
    public string Name { get; set; } = string.Empty;
}