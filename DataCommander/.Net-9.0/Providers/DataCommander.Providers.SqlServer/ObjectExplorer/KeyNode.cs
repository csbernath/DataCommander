using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal class KeyNode : ITreeNode
{
    private readonly string? _name;

    public KeyNode(DatabaseNode databaseNode, int id, string? name)
    {
        _name = name;
    }

    public string? Name => _name;
    public bool IsLeaf => true;

    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken) =>
        Task.FromResult<IEnumerable<ITreeNode>>(Array.Empty<ITreeNode>());

    public bool Sortable => false;
    public string Query => null;
    public ContextMenu? GetContextMenu() => null;
}