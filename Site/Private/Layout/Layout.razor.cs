using Microsoft.AspNetCore.Components;
using Site.Shared.Auth;

namespace Site.Private.Layout;

public partial class Layout
{
    [Inject]
    public AuthStore AuthStore { get; set; } = default!;

    [Inject]
    public TokensStore TokensStore { get; set; } = default!;

    [Inject]
    public NavigationManager Navigation { get; set; } = default!;

    private string _user = string.Empty;

    protected override void OnInitialized()
    {
        var credentials = AuthStore.TryLoadCredentials();

        if (credentials is null)
            Navigation.NavigateTo(Routes.Login);
        else
            _user = credentials.User;
    }

    private void Logout()
    {
        AuthStore.ClearCredentials();
        TokensStore.Clear();
        Navigation.NavigateTo(Routes.Login);
    }
}