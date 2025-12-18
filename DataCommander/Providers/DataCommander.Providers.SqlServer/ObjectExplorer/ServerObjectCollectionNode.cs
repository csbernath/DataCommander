using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class ServerObjectCollectionNode : ITreeNode
{
    private readonly ServerNode _server;

    public ServerObjectCollectionNode(ServerNode serverNode)
    {
        ArgumentNullException.ThrowIfNull(serverNode);
        _server = serverNode;
    }

    string? ITreeNode.Name => "Server Objects";

    bool ITreeNode.IsLeaf => false;

    public IReadOnlyCollection<string> GetFilterableProperties() => [];

    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(IReadOnlyList<FilterCriterion> filterCriteria, bool refresh, CancellationToken cancellationToken) =>
        Task.FromResult<IEnumerable<ITreeNode>>([new LinkedServerCollectionNode(_server)]);

    bool ITreeNode.Sortable => false;

    public bool DynamicChildCount => false;

    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken) => Task.FromResult<string?>(null);

    public ContextMenu? GetContextMenu() => null;
}