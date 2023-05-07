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
        var credentials = AuthStore.LoadCredentials();
        Navigation.NavigateTo(string.IsNullOrWhiteSpace(credentials) ? Routes.Login : Routes.Dashboard);
    }
}