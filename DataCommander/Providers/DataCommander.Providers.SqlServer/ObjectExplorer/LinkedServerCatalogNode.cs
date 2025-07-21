using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class LinkedServerCatalogNode : ITreeNode
{
    private readonly string? _name;

    public LinkedServerCatalogNode(LinkedServerNode linkedServer, string? name)
    {
        ArgumentNullException.ThrowIfNull(linkedServer);
        _name = name;
    }

    string? ITreeNode.Name => _name;

    bool ITreeNode.IsLeaf => true;

    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken) => throw new NotImplementedException();

    bool ITreeNode.Sortable => false;

    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken) => Task.FromResult<string?>(null);

    public ContextMenu? GetContextMenu() => null;
}