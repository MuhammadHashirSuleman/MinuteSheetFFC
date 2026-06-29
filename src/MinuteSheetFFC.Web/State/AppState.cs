using MinuteSheetFFC.Application.DTOs.Auth;

namespace MinuteSheetFFC.Web.State;

public class AppState
{
    public UserProfileDto? CurrentUser { get; private set; }
    public string? AccessToken { get; private set; }
    public string? RefreshToken { get; private set; }
    public bool IsAuthenticated => CurrentUser != null && !string.IsNullOrEmpty(AccessToken);
    public event Action? OnChange;

    public void SetAuth(AuthResponseDto auth)
    {
        CurrentUser = auth.User;
        AccessToken = auth.AccessToken;
        RefreshToken = auth.RefreshToken;
        NotifyStateChanged();
    }

    public void ClearAuth()
    {
        CurrentUser = null;
        AccessToken = null;
        RefreshToken = null;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
