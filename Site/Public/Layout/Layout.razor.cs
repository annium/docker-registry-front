using Microsoft.AspNetCore.Components;
using Site.Shared.Auth;

namespace Site.Public.Layout;

public partial class Layout
{
    [Inject]
    public AuthStore AuthStore { get; set; } = default!;

    [Inject]
    public NavigationManager Navigation { get; set; } = default!;

    protected override void OnInitialized()
    {
        if (AuthStore.HasCredentials())
            Navigation.NavigateTo(Routes.Dashboard);
    }
}