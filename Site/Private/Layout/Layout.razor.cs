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
        var credentials = AuthStore.LoadCredentials();

        if (string.IsNullOrWhiteSpace(credentials))
            Navigation.NavigateTo(Routes.Login);
    }
}