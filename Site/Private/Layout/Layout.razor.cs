using Microsoft.AspNetCore.Components;
using Site.Shared.Auth;

namespace Site.Private.Layout;

public partial class Layout
{
    [Inject]
    public AuthStore AuthStore { get; set; } = default!;

    [Inject]
    public NavigationManager Navigation { get; set; } = default!;

    protected override void OnInitialized()
    {
        if (!AuthStore.HasCredentials())
            Navigation.NavigateTo(Routes.Login);
    }

    private void Logout()
    {
        AuthStore.ClearCredentials();
        Navigation.NavigateTo(Routes.Login);
    }
}