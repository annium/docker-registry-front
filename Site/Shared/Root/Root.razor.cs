using Microsoft.AspNetCore.Components;
using Site.Shared.Auth;

namespace Site.Shared.Root;

public partial class Root
{
    [Inject]
    public AuthStore AuthStore { get; set; } = default!;

    [Inject]
    public NavigationManager Navigation { get; set; } = default!;

    protected override void OnInitialized()
    {
        Navigation.NavigateTo(AuthStore.HasCredentials() ? Routes.Repositories : Routes.Login);
    }
}
