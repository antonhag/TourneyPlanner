using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace TourneyPlanner.UI.Services;

// håller inloggningen och sparar den krypterat i webbläsaren,
// hindrar att man loggas ut vid refresh av sidan
public class AuthState
{
    private const string AuthStateKey = "authState";
    private readonly ProtectedLocalStorage _storage;
    private bool _restored;

    public AuthState(ProtectedLocalStorage storage)
    {
        _storage = storage;
    }
    
    public string? AccessToken { get; private set; }
    public string? Email { get; private set; }

    public bool IsLoggedIn => !string.IsNullOrEmpty(AccessToken);
    
    public event Action? OnChange;

    public async Task LoginAsync(string email, string accessToken)
    {
        Email = email;
        AccessToken = accessToken;
        await _storage.SetAsync(AuthStateKey, new SavedLogin
        {
            Email = email,
            AccessToken = accessToken
        });
        
        OnChange?.Invoke();
    }

    public async Task LogoutAsync()
    {
        Email = null;
        AccessToken = null;
        await _storage.DeleteAsync(AuthStateKey);
        OnChange?.Invoke();
    }

    // läser tillbaka en sparad inloggning
    public async Task RestoreAsync()
    {
        if (_restored)
        {
            return;
        }

        _restored = true;

        try
        {
            var result = await _storage.GetAsync<SavedLogin>(AuthStateKey);

            if (result.Success && result.Value != null)
            {
                Email = result.Value.Email;
                AccessToken = result.Value.AccessToken; 
                OnChange?.Invoke();
            }
        }
        catch
        {
            // ifall sparad inlogg ej funkar, tas den bort och man loggas ut
            await _storage.DeleteAsync(AuthStateKey);
        }
    }

    // detta sparas i webbläsaren
    public class SavedLogin
    {
        public string Email { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
    }
    
}