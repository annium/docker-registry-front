using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Site.Shared.Api;

namespace Site.Private.Repositories;

public partial class RepositoriesPage
{
    [Inject]
    private Api Api { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        await Api.ListRepositoriesAsync();
    }
}