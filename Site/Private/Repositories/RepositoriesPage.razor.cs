using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Site.Shared.Api;
using Site.Shared.Api.Dto;

namespace Site.Private.Repositories;

public partial class RepositoriesPage
{
    [Inject]
    private Api Api { get; set; } = default!;

    private bool _isLoading = true;
    private IReadOnlyList<Repository> _repositories = Array.Empty<Repository>();

    protected override async Task OnInitializedAsync()
    {
        _repositories = await Api.GetCatalogAsync();
        _isLoading = false;
    }
}