using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AntDesign.TableModels;
using Microsoft.AspNetCore.Components;
using Site.Shared.Api;
using Site.Shared.Api.Dto;

namespace Site.Private.Repositories;

public partial class RepositoriesPage
{
    [Inject]
    private Api Api { get; set; } = default!;

    private bool _isRepositoriesTableLoading = true;
    private bool _isTagsTableLoading;
    private IReadOnlyList<Repository> _repositories = Array.Empty<Repository>();
    private readonly Dictionary<Repository, IReadOnlyList<RepositoryTag>> _repositoryTags = new();

    protected override async Task OnInitializedAsync()
    {
        _repositories = await Api.GetCatalogAsync();
        _isRepositoriesTableLoading = false;
    }

    private IReadOnlyList<RepositoryTag> GetRepositoryTags(Repository repository)
    {
        return _repositoryTags.TryGetValue(repository, out var tags) ? tags : Array.Empty<RepositoryTag>();
    }

    private async Task LoadRepositoryTagsAsync(RowData<Repository> row)
    {
        if (_repositoryTags.ContainsKey(row.Data))
            return;

        _isTagsTableLoading = true;
        _repositoryTags[row.Data] = await Api.ListTagsAsync(row.Data);
        _isTagsTableLoading = false;
    }
}