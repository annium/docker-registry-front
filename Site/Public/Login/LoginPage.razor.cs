using System.Threading.Tasks;
using AntDesign;
using Microsoft.AspNetCore.Components;
using Site.Shared.Api.Server;

namespace Site.Public.Login;

public partial class LoginPage
{
    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    [Inject]
    private IMessageService Message { get; set; } = default!;

    [Inject]
    private ServerApi ServerApi { get; set; } = default!;

    private string User { get; set; } = string.Empty;
    private string Password { get; set; } = string.Empty;
    private bool _isLoading;

    private async Task Submit()
    {
        _isLoading = true;
        StateHasChanged();

        var result = await ServerApi.LoginAsync(User, Password);

        _isLoading = false;
        StateHasChanged();

        if (result)
            Navigation.NavigateTo(Routes.Dashboard);
        else
            await Message.Error("Incorrect login os password", 1);
    }
}