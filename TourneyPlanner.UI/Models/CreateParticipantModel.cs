using System.ComponentModel.DataAnnotations;

namespace TourneyPlanner.UI.Models;

public class CreateParticipantModel
{
    [Required(ErrorMessage = "Namn krävs")]
    [MaxLength(25, ErrorMessage = "namnet får högst vara 25 tecken")]
    public string Name { get; set; } = string.Empty;
}
