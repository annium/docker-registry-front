using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AntDesign;
using AntDesign.TableModels;
using Microsoft.AspNetCore.Components;
using Site.Shared.Api;
using Site.Shared.Api.Dto;

namespace Site.Private.Repositories;

public partial class RepositoriesPage
{
    [Inject]
    private Api Api { get; set; } = default!;

    [Inject]
    private IConfirmService Confirm { get; set; } = default!;

    [Inject]
    private IMessageService Message { get; set; } = default!;

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
        _repositoryTags[row.Data] = await Api.ListTagsAsync(row.Data.Name);
        _isTagsTableLoading = false;
    }

    private async Task DeleteRepositoryTagAsync(Repository repository, RepositoryTag tag)
    {
        var answer = await Confirm.Show("Operation is irreversible.", $"Delete image {repository.Name}:{tag.Name}?", ConfirmButtons.YesNo, ConfirmIcon.Warning);
        if (answer is not ConfirmResult.Yes)
            return;

        _isTagsTableLoading = true;
        StateHasChanged();

        var warning = Message.Warning($"Deleting image {repository.Name}:{tag.Name}...", 2);
        var result = await Api.DeleteTagAsync(repository.Name, tag.Digest);
        await warning;

        if (result)
        {
            _repositoryTags[repository] = await Api.ListTagsAsync(repository.Name);
            _isTagsTableLoading = false;

            StateHasChanged();
            await Message.Success($"Deleted image {repository.Name}:{tag.Name}", 2);
        }
        else
        {
            _isTagsTableLoading = false;
            StateHasChanged();
            await Message.Error($"Failed to delete image {repository.Name}:{tag.Name}", 2);
        }
    }
}