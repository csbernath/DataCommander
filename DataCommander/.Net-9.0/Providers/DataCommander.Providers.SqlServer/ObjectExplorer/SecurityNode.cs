using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Linq;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class SecurityNode : ITreeNode
{
    private readonly ServerNode _server;

    public SecurityNode(ServerNode serverNode)
    {
        ArgumentNullException.ThrowIfNull(serverNode);
        _server = serverNode;
    }

    string? ITreeNode.Name => "Security";

    bool ITreeNode.IsLeaf => false;

    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken) =>
        Task.FromResult<IEnumerable<ITreeNode>>(new LoginCollectionNode(_server).ItemToArray());

    bool ITreeNode.Sortable => false;

    string? ITreeNode.Query => null;

    public ContextMenu? GetContextMenu() => null;
}