using System.ComponentModel.DataAnnotations;

namespace TourneyPlanner.UI.Models;

public class UpdateTournamentModel
{
    [Required(ErrorMessage = "Namn krävs")]
    [MaxLength(15, ErrorMessage = "Namn får högst vara 15 tecken.")]
    public string Name { get; set; } = string.Empty;
    
    public DateTime StartDate { get; set; }
    
    public DateTime EndDate { get; set; }

    [Range(2, int.MaxValue, ErrorMessage = "Minst 2 deltagare")]
    public int Size { get; set; } = 2;
}