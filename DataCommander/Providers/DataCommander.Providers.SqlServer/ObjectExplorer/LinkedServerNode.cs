using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class LinkedServerNode : ITreeNode
{
    public LinkedServerNode(LinkedServerCollectionNode linkedServers, string? name)
    {
        ArgumentNullException.ThrowIfNull(linkedServers);
        LinkedServers = linkedServers;
        Name = name;
    }

    public LinkedServerCollectionNode LinkedServers { get; }
    public string? Name { get; }
    string? ITreeNode.Name => Name;
    bool ITreeNode.IsLeaf => false;

    public IReadOnlyCollection<string> GetFilterableProperties() => [];

    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(IReadOnlyCollection<FilterCriterion> filterCriteria, bool refresh, CancellationToken cancellationToken) =>
        Task.FromResult<IEnumerable<ITreeNode>>([new LinkedServerCatalogCollectionNode(this)]);

    public IReadOnlyCollection<FilterCriterion> GetFilterCriteria() => [];

    bool ITreeNode.Sortable => false;

    public bool DynamicChildCount => true;

    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken) => Task.FromResult<string?>(null);

    public ContextMenu? GetContextMenu() => null;
}