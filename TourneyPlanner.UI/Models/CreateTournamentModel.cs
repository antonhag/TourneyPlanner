using System.ComponentModel.DataAnnotations;

namespace TourneyPlanner.UI.Models;

public class CreateTournamentModel
{
    [Required(ErrorMessage = "Namn krävs")]
    [MaxLength(15, ErrorMessage = "namnet får högst vara 15 tecken")]
    public string Name { get; set; } = string.Empty;
    
    public DateTime StartDate { get; set; } = DateTime.Today;
    

    public int Size { get; set; } = 2;

}