using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Site.Shared.Api.Server;

namespace Site.Public.Login;

public partial class LoginPage
{
    [Inject]
    private ServerApi ServerApi { get; set; } = default!;

    private string User { get; set; } = string.Empty;
    private string Password { get; set; } = string.Empty;

    private async Task Submit()
    {
        var result = await ServerApi.LoginAsync(User, Password);
    }
}