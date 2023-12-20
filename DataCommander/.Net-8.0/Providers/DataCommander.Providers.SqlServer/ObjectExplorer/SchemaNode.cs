using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class SchemaNode : ITreeNode
{
    private readonly DatabaseNode _database;

    public SchemaNode(DatabaseNode database, string name)
    {
        _database = database;
        Name = name;
    }

    public string Name { get; }
    public bool IsLeaf => true;

    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken) =>
        Task.FromResult<IEnumerable<ITreeNode>>(Array.Empty<ITreeNode>());

    public bool Sortable => false;
    public string Query => null;

    public ContextMenu? GetContextMenu() => null;
}