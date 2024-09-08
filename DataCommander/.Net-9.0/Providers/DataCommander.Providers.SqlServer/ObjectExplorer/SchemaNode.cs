using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class SchemaNode(DatabaseNode database, string? name) : ITreeNode
{
    private readonly DatabaseNode _database = database;

    public string? Name { get; } = name;
    public bool IsLeaf => true;

    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken) =>
        Task.FromResult<IEnumerable<ITreeNode>>(Array.Empty<ITreeNode>());

    public bool Sortable => false;
    public string Query => null;

    public ContextMenu? GetContextMenu() => null;
}