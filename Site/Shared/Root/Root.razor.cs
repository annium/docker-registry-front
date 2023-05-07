using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;

namespace Site.Shared.Root;

public partial class Root
{
    [Inject]
    public IConfiguration Config { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        Console.WriteLine($"Root initialized: {Config["registry"]}");
        await Task.CompletedTask;
    }
}