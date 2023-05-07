using System;
using System.Threading.Tasks;

namespace Site.Public.Login;

public partial class LoginPage
{
    private string User { get; set; } = string.Empty;
    private string Password { get; set; } = string.Empty;

    private async Task Submit()
    {
        Console.WriteLine($"Log in as {User}:{Password}");
        await Task.CompletedTask;
    }
}