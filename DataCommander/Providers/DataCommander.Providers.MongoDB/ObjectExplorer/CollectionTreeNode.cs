using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;

namespace DataCommander.Providers.MongoDB.ObjectExplorer;

public class CollectionTreeNode(
    DatabaseTreeNode databaseTreeNode,
    string name) : ITreeNode
{
    public string? Name => name;

    public bool IsLeaf => true;

    public IReadOnlyCollection<string> GetFilterableProperties() => throw new System.NotImplementedException();

    public Task<IEnumerable<ITreeNode>> GetChildren(IReadOnlyCollection<FilterCriterion> filterCriteria, bool refresh, CancellationToken cancellationToken) =>
        throw new System.NotImplementedException();

    public IReadOnlyCollection<FilterCriterion> GetFilterCriteria() => [];

    public bool Sortable => false;

    public bool DynamicChildCount => throw new System.NotImplementedException();

    public Task<string?> GetQuery(CancellationToken cancellationToken) => throw new System.NotImplementedException();

    public ContextMenu? GetContextMenu()
    {
        IReadOnlyCollection<MenuItem> menuItems =
        [
            new("Set as current collection", SetAsCurrentCollectionMenuItemClicked, [])
        ];
        return new ContextMenu(menuItems);
    }

    private void SetAsCurrentCollectionMenuItemClicked(object? sender, EventArgs e) =>
        databaseTreeNode.MongoObjectExplorer.SetCurrentDatabaseAndCollectionName(databaseTreeNode.Name, name);
}