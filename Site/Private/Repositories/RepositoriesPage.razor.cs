using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Site.Shared.Api.Registry;

namespace Site.Private.Repositories;

public partial class RepositoriesPage
{
    [Inject]
    private RegistryApi RegistryApi { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        await RegistryApi.ListRepositoriesAsync();
    }
}