using System.ComponentModel.DataAnnotations;

namespace TourneyPlanner.UI.Models;

public class LoginModel
{
    [Required(ErrorMessage = "E-post krävs")]
    [EmailAddress(ErrorMessage = "Ogiltigt E-post")]
    public string Email { get; set; } = String.Empty;
    
    [Required(ErrorMessage = "Lösenord krävs")]
    public string Password { get; set; } = String.Empty;
}
