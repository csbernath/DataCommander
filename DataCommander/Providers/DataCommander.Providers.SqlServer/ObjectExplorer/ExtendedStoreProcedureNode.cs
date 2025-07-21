using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class ExtendedStoreProcedureNode(DatabaseNode database, string schema, string name) : ITreeNode
{
    private readonly DatabaseNode _database = database;

    string? ITreeNode.Name => $"{schema}.{name}";
    bool ITreeNode.IsLeaf => true;
    bool ITreeNode.Sortable => false;
    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken) => Task.FromResult<string?>(null);
    public ContextMenu? GetContextMenu() => null;

    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken) => throw new NotSupportedException();
}