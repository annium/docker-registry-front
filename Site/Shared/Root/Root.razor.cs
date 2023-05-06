using System;
using System.Threading.Tasks;

namespace Site.Shared.Root;

public partial class Root
{
    protected override async Task OnInitializedAsync()
    {
        Console.WriteLine("Root initialized");
        await Task.CompletedTask;
    }
}