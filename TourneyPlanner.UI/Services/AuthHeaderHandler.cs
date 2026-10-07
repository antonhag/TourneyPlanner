using System.Net.Http.Headers;

namespace TourneyPlanner.UI.Services;


public class AuthHeaderHandler : DelegatingHandler
{
    private readonly AuthState _authState;
    public AuthHeaderHandler(AuthState authState)
    {
        _authState = authState;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (_authState.IsLoggedIn)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authState.AccessToken);
        }
        return base.SendAsync(request, cancellationToken);
    }

}