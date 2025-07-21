using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class DatabaseSecurityNode : ITreeNode
{
    private readonly DatabaseNode _databaseNode;

    public DatabaseSecurityNode(DatabaseNode databaseNode)
    {
        ArgumentNullException.ThrowIfNull(databaseNode);
        _databaseNode = databaseNode;
    }

    string? ITreeNode.Name => "Security";

    bool ITreeNode.IsLeaf => false;

    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken) => Task.FromResult<IEnumerable<ITreeNode>>(
        [
            new UserCollectionNode(_databaseNode),
            new RoleCollectionNode(_databaseNode),
            new SchemaCollectionNode(_databaseNode)
        ]);

    bool ITreeNode.Sortable => false;

    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken) => Task.FromResult<string?>(null);

    public ContextMenu? GetContextMenu() => null;
}