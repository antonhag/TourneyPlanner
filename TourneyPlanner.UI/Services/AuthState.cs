namespace TourneyPlanner.UI.Services;

public class AuthState
{
    public string? AccessToken { get; private set; }
    public string? Email { get; private set; }

    public bool IsLoggedIn => !string.IsNullOrEmpty(AccessToken);
    
    public event Action? OnChange;

    public void Login(string email, string accessToken)
    {
        Email = email;
        AccessToken = accessToken;
        OnChange?.Invoke();
    }

    public void Logout()
    {
        Email = null;
        AccessToken = null;
        OnChange?.Invoke();
    }
    
}